using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/report")]
    public class ReportController : ApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;

        public ReportController(IUnitOfWork uow, INotificationService notificationService, IMembershipService membershipService)
        {
            _uow = uow;
            _notificationService = notificationService;
            _membershipService = membershipService;
        }

        [HttpGet, Route("{datayear:int}/{filenumber}")]
        public async Task<object> Get(int datayear, string filenumber)
        {
            var reports = await _uow.Reports.GetByFileSpecificationAsync(datayear, filenumber);
            var dto = Mapper.Map<List<ReportDto>>(reports);
            //foreach (var reportDto in dto)
            //{
            //    var reportDocumentDtos = reportDto.Documents.OrderByDescending(x => x.Version);
            //    reportDto.Documents = reportDocumentDtos.ToList();
            //}
            return Ok(dto);
        }

        [HttpPost, Route("create/{id}")]
        public async Task<object> Create(int id)
        {

            var submission = await _uow.Submissions.GetByIdAsync(id);

            if (submission == null) return NotFound();

            var report = Report.Create(submission.DataYear);

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
        public async Task<object> Waiver(int id)
        {
            var submission = await _uow.Submissions.GetByIdAsync(id);
            if (submission == null) return NotFound();

            var report = Report.Create(submission.DataYear);

            submission.AddReport(report);

            submission.Waive();
            report.Waive();

            await _uow.CompleteAsync();

            var dto = Mapper.Map<ReportDto>(report);

            return Ok(dto);
        }

        [HttpPost, Route("cancel/{id}")]
        public async Task<object> Cancel(int id)
        {
            var submission = await _uow.Submissions.GetByIdAsync(id);
            if (submission == null) return NotFound();

            var report = await _uow.Reports.GetBySubmissionIdAsync(id);
            //Delete work items 
            _uow.WorkItems.DeleteFromReport(report.Id);

            //Delete documents
            _uow.Documents.DeleteReportDocuments(report.Id);

            //Delete reports
            _uow.Reports.Delete(report.Id);

            //Set submission state
            submission.SetState(WorkItemAction.Nothing);

            await _uow.CompleteAsync();

            return Ok();
        }

    }
}
