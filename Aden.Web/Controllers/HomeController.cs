using Aden.Core.Data;
using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Web.Filters;
using Aden.Web.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EAGetMail;
//using Independentsoft.Email.Mime;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Z.EntityFramework.Plus;


namespace Aden.Web.Controllers
{
    //[Authorize]
    [CustomAuthorize(Roles = "AdenAppUsers")]
    public class HomeController : AsyncController
    {
        private readonly AdenContext _context;
        private readonly IUnitOfWork _uow;

        public HomeController(AdenContext context, IUnitOfWork uow)
        {
            _context = context;
            _uow = uow;
        }

        [TrackViewName]
        [CustomAuthorize(Roles = Constants.FileSpecificationAdministratorGroup)]
        public ActionResult FileSpecifications()
        {
            return View();
        }

        [TrackViewName]
        public ActionResult Submissions()
        {
            return View();
        }

        [TrackViewName]
        public async Task<ActionResult> Reports(int datayear, string filenumber)
        {
            var reports = await _context.Reports
                .Include(f => f.Submission.FileSpecification)
                .Include(r => r.Documents)
                .Where(f => (f.Submission.FileSpecification.FileNumber == filenumber && f.Submission.DataYear == datayear) || string.IsNullOrEmpty(filenumber))
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            var dto = Mapper.Map<List<ReportViewDto>>(reports);
            return View(dto);
        }

        [TrackViewName]
        public ActionResult Assignments(string view, string username)
        {
            return View((object)User.Identity.Name);
        }

        public async Task<ActionResult> WorkHistory(int submissionId)
        {
            var isAdministrator = ((ClaimsPrincipal)User).Claims.Any(c => c.Value.ToLower().Contains(Constants.GlobalAdministratorGroup.ToLower()));

            ViewBag.IsSectionAdmin = isAdministrator;

            var dto = new SubmissionWorkHistoryViewDto()
            {
                WorkItemHistory = new List<WorkItemHistoryDto>(),
                SubmissionAudits = new List<SubmissionAudit>()
            };

            var report = _context.Reports.OrderByDescending(r => r.Id).FirstOrDefault(x => x.SubmissionId == submissionId);

            if (report == null) return PartialView("_WorkHistory", dto);

            dto.WorkItemHistory = _context.WorkItems.OrderByDescending(o => o.Id).Where(w => w.ReportId == report.Id)
                .ProjectTo<WorkItemHistoryDto>().Future().ToList();
            dto.SubmissionAudits = _context.SubmissionAudits.OrderByDescending(x => x.Id).Where(a => a.SubmissionId == submissionId).Future().ToList();

            return PartialView("_WorkHistory", dto);

        }

        public async Task<ActionResult> WorkItemImages(int workItemId)
        {
            var wi = await _context.WorkItems.Include(x => x.WorkItemImages).FirstOrDefaultAsync(x => x.Id == workItemId);

            return PartialView("_WorkItemImage", wi);
        }

        public async Task<ActionResult> Reassign(int workItemId)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(workItemId);

            var dto = Mapper.Map<ReassignmentDto>(workItem);

            return PartialView("_WorkItemReassignment", dto);
        }

        public async Task<ActionResult> Document(int id)
        {
            var document = await _uow.Documents.GetByIdAsync(id);

            var dto = Mapper.Map<ReportDocumentDto>(document);

            return PartialView(dto);
        }

        public async Task<FileResult> Download(int id)
        {
            var document = await _uow.Documents.GetByIdAsync(id);
            return File(document.FileData, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }

        public ActionResult EditFileSpecification(int id)
        {
            var spec = _uow.FileSpecifications.GetById(id);

            var model = Mapper.Map<UpdateFileSpecificationDto>(spec);
            return PartialView("_FileSpecificationForm", model);

        }

        public async Task<ActionResult> ErrorReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<WorkItemDto>(wi);
            return PartialView("_ErrorReportForm", model);
        }

        public async Task<ActionResult> UploadReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<ReportUploadDto>(wi);
            return PartialView("_ReportUploadForm", model);
        }

        public ActionResult Error(string message)
        {
            return View();
        }

        public ActionResult Audit(int id)
        {
            var audit = new SubmissionAuditEntryDto() { SubmissionId = id };
            return PartialView("_SubmissionAuditEntry", audit);
        }

        public ActionResult ReopenAudit(int id)
        {
            var audit = new SubmissionReopenAuditEntryDto() { SubmissionId = id };
            return PartialView("_SubmissionReopenAuditEntry", audit);
        }

        [TrackViewName]
        public ActionResult Mail()
        {
            var vm = new List<MailViewModel>();
            var path = HostingEnvironment.MapPath(@"/App_Data");
            foreach (var file in Directory.GetFiles($@"{path}", "*.eml"))
            {

                var msg = new Mail("TryIt");
                msg.Load(file, false);
                vm.Add(new MailViewModel()
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Sent = msg.ReceivedDate,
                    To = msg.To.Select(s => s.Address.ToString()),
                    CC = msg.Cc.Select(s => s.Address.ToString()),

                    From = msg.From.Address,
                    Subject = msg.Subject.Replace("(Trial Version)", ""),
                    Body = msg.HtmlBody,
                    Attachments = msg.Attachments.ToList()
                });
            }

            return View(vm.OrderByDescending(x => x.Sent));
        }

    }
}