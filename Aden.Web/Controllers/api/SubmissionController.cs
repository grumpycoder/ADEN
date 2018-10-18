using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using Aden.Web.ViewModels;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/submission")]
    public class SubmissionController : ApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;

        public SubmissionController(IUnitOfWork uow, INotificationService notificationService, IMembershipService membershipService)
        {
            _uow = uow;
            _notificationService = notificationService;
            _membershipService = membershipService;
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            //TODO: Refactor isGlobalAdmin variable
            var isGlobalAdmin = User.IsInRole(Constants.GlobalAdministratorGroup);

            //TODO: Refactor to use a custom claimtype and not magic string
            var section = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "Section")?.Value;

            var submissions = await _uow.Submissions.GetBySectionWithReportsAsync(!isGlobalAdmin ? section : string.Empty);

            var rows = Mapper.Map<List<SubmissionDto>>(submissions);


            return Ok(DataSourceLoader.Load(rows, loadOptions));

        }

        [HttpPost, Route("reopen/{id}")]
        public async Task<object> Reopen(int id, SubmissionAuditEntryDto model)
        {
            var submission = await _uow.Submissions.GetByIdAsync(model.SubmissionId);

            if (submission == null) return NotFound();

            var report = Report.Create(submission.DataYear);

            if (submission.SubmissionState != SubmissionState.NotStarted)
            {
                submission.Reopen(model.Message, User.Identity.Name);
            }
            submission.AddReport(report);

            //Get assignee
            var members = _membershipService.GetGroupMembers(submission.FileSpecification.GenerationUserGroup);
            if (members.IsFailure) return BadRequest(members.Error);

            var assignee = _uow.WorkItems.GetUserWithLeastAssignments(members.Value);

            var workItem = WorkItem.Create(WorkItemAction.Generate, assignee);

            report.AddWorkItem(workItem);
            report.SetState(workItem.WorkItemAction);
            submission.SetState(workItem.WorkItemAction);

            await _uow.CompleteAsync();

            _notificationService.SendWorkNotification(workItem);

            var dto = Mapper.Map<ReportDto>(report);
            return Ok(dto);
        }

        [HttpPost, Route("waiver/{id}")]
        public async Task<object> Waiver(int id, SubmissionAuditEntryDto model)
        {
            var submission = await _uow.Submissions.GetByIdAsync(id);
            if (submission == null) return NotFound();

            var report = Report.Create(submission.DataYear);

            submission.AddReport(report);

            submission.Waive(model.Message, User.Identity.Name);
            report.Waive();

            await _uow.CompleteAsync();

            var dto = Mapper.Map<ReportDto>(report);

            return Ok(dto);
        }
    }
}
