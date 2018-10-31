using Aden.Core.Data;
using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using Aden.Web.ViewModels;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/submission")]
    public class SubmissionController : ApiController
    {
        private readonly AdenContext _context;
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;

        public SubmissionController(AdenContext context, IUnitOfWork uow, INotificationService notificationService, IMembershipService membershipService)
        {
            _context = context;
            _uow = uow;
            _notificationService = notificationService;
            _membershipService = membershipService;
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            //TODO: Refactor to use a custom ClaimType and not magic string
            var section = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "Section")?.Value;

            var s = await _context.Submissions.ToListAsync();

            var dto = await _context.Submissions
                .ProjectTo<SubmissionViewDto>().ToListAsync();

            return Ok(DataSourceLoader.Load(dto.OrderBy(x => x.DueDate).ThenByDescending(x => x.Id), loadOptions));
        }

        [HttpPost, Route("start/{submissionId}")]
        public async Task<object> Start(int submissionId)
        {

            var submission = await _context.Submissions.Include(f => f.FileSpecification)
                .FirstOrDefaultAsync(s => s.Id == submissionId);
            if (submission == null) return NotFound();

            if (string.IsNullOrWhiteSpace(submission.FileSpecification.GenerationUserGroup))
                return BadRequest("No Generation Group defined for specification");


            submission.Start(User.Identity.Name);

            //HACK: Instructed to do by management
            if (submission.FileSpecification.ReportAction == "manual")
            {
                var groupName = "Data Collections";
                if (submission.FileSpecification.SupportGroup == "Development")
                    groupName = "IS Programmers Software Developers";

                var client = new SmtpClient();
                var message = new MailMessage()
                {
                    Body = $"File needs to be created for {submission.FileSpecification.FileName}.<br /> Please assign to {groupName} group",
                    To = { "helpdesk@alsde.edu" },
                    From = new MailAddress(User.Identity.Name),
                    IsBodyHtml = true
                };

                client.Send(message);

            }

            var report = submission.CreateReport();

            //Get assignee
            var members = _membershipService.GetGroupMembers(submission.FileSpecification.GenerationUserGroup);
            if (members.IsFailure) return BadRequest(members.Error);
            var assignee = _uow.WorkItems.GetUserWithLeastAssignments(members.Value);

            var wi = report.CreateTask(assignee);

            _context.SaveChanges();

            _notificationService.SendWorkNotification(wi);

            return Ok();

        }

        [HttpPost, Route("cancel/{submissionId}")]
        public async Task<object> Cancel(int submissionId)
        {
            var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.Id == submissionId);
            if (submission == null) return NotFound();

            submission.Cancel(User.Identity.Name);
            //TODO: Add to Cancel method

            var report = await _context.Reports.FirstOrDefaultAsync(r => r.SubmissionId == submissionId);
            if (report != null)
            {
                var wi = _context.WorkItems.Where(w => w.ReportId == report.Id);
                _context.WorkItems.RemoveRange(wi);

                var docs = _context.ReportDocuments.Where(d => d.ReportId == report.Id);
                _context.ReportDocuments.RemoveRange(docs);

                _context.Reports.Remove(report);
            }

            submission.SubmissionState = SubmissionState.NotStarted;

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost, Route("reopen/{id}")]
        public async Task<object> Reopen(int id, SubmissionReopenAuditEntryDto model)
        {
            var submission = await _context.Submissions.Include(f => f.FileSpecification).FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null) return NotFound();

            var report = submission.CreateReport();

            //Get assignee
            var members = _membershipService.GetGroupMembers(submission.FileSpecification.GenerationUserGroup);
            if (members.IsFailure) return BadRequest(members.Error);

            var assignee = _uow.WorkItems.GetUserWithLeastAssignments(members.Value);

            var wi = report.CreateTask(assignee);

            submission.Reopen(model.Message, User.Identity.Name);

            submission.NextDueDate = model.NextSubmissionDate;

            _context.SaveChanges();

            _notificationService.SendWorkNotification(wi);

            return Ok("Successfully reopened Submission");
        }

        [HttpPost, Route("waiver/{id}")]
        public async Task<object> Waiver(int id, SubmissionAuditEntryDto model)
        {
            var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null) return NotFound();

            var report = submission.CreateReport();

            submission.Waive(model.Message, User.Identity.Name);

            report.Waive();

            _context.SaveChanges();

            return Ok("Successfully waived Submission");
        }
    }


}
