using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using AutoMapper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/assignment")]
    [Authorize]
    public class AssignmentController : ApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;

        public AssignmentController(IUnitOfWork uow, INotificationService notificationService, IMembershipService membershipService)
        {
            _uow = uow;
            _notificationService = notificationService;
            _membershipService = membershipService;
        }

        [HttpGet, Route("current/{username}")]
        public async Task<object> CurrentAssignments(string username)
        {
            if (username == null) return NotFound();
            var workitems = await _uow.WorkItems.GetActiveAsync(username);

            var dto = Mapper.Map<List<WorkItemDto>>(workitems);

            return Ok(dto);
        }

        [HttpGet, Route("history/{username}")]
        public async Task<object> History(string username)
        {
            //TODO: Should use authenticated user?
            var workItems = await _uow.WorkItems.GetCompletedAsync(username);

            var dto = Mapper.Map<List<WorkItemDto>>(workItems.OrderByDescending(w => w.CanCancel).ThenByDescending(w => w.AssignedDate));

            return Ok(dto);
        }

        [HttpPost, Route("complete/{id}")]
        public async Task<object> Complete(int id)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(id);

            if (workItem == null) return NotFound();

            if (workItem.WorkItemAction == WorkItemAction.Generate)
            {
                //Create documents
                var createOrError = await _uow.GenerateDocumentsAsync(workItem.ReportId);
                if (createOrError.IsFailure) return BadRequest(createOrError.Error);
            }

            workItem.Finish();

            //Start new work item
            var report = await _uow.Reports.GetByIdAsync(workItem.ReportId);

            //What's the next work
            var next = WorkItem.Next(workItem);
            if (next == WorkItemAction.Nothing)
            {
                report.SetState(next);
                report.Submission.SetState(next);
                await _uow.CompleteAsync();
                return Ok();
            }

            //Get assignee
            var groupName = report.Submission.FileSpecification.GenerationUserGroup;
            switch (next)
            {
                case WorkItemAction.Approve:
                    groupName = report.Submission.FileSpecification.ApprovalUserGroup;
                    break;
                case WorkItemAction.Submit:
                    groupName = report.Submission.FileSpecification.SubmissionUserGroup;
                    break;
            }
            var members = _membershipService.GetGroupMembers(groupName);
            if (members.IsFailure) return BadRequest(members.Error);

            var assignee = _uow.WorkItems.GetUserWithLeastAssignments(members.Value);

            var nextWorkItem = WorkItem.Create(next, assignee);

            report.SetState(next);
            report.Submission.SetState(next);

            report.AddWorkItem(nextWorkItem);

            await _uow.CompleteAsync();

            //Send work notification
            _notificationService.SendWorkNotification(nextWorkItem);

            var dto = Mapper.Map<WorkItemDto>(nextWorkItem);
            return Ok(dto);
        }

        [HttpPost, Route("cancel/{id}")]
        public async Task<object> Cancel(int id)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(id);

            if (workItem == null) return NotFound();

            var report = await _uow.Reports.GetByIdAsync(workItem.ReportId);

            report.CancelWorkItems();

            report.SetState(WorkItemAction.Nothing);
            report.Submission.SetState(WorkItemAction.Nothing);

            //Get assignee
            var members = _membershipService.GetGroupMembers(report.Submission.FileSpecification.GenerationUserGroup);
            if (members.IsFailure) return BadRequest(members.Error);

            var assignee = _uow.WorkItems.GetUserWithLeastAssignments(members.Value);

            var newWorkItem = WorkItem.Create(WorkItemAction.Generate, assignee);

            report.AddWorkItem(newWorkItem);
            report.SetState(newWorkItem.WorkItemAction);
            report.Submission.SetState(newWorkItem.WorkItemAction);

            await _uow.CompleteAsync();

            //Send cancel notification
            _notificationService.SendWorkCancelNotification(workItem);

            //Send work notification
            _notificationService.SendWorkNotification(newWorkItem);

            var dto = Mapper.Map<WorkItemDto>(newWorkItem);
            return Ok(dto);
        }

        [HttpPost, Route("reassign")]
        public async Task<object> Reassign([FromBody]ReassignmentDto model)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(model.WorkItemId);
            if (workItem == null) return NotFound();

            //Send work reassignment notification
            _notificationService.SendWorkReassignmentNotification(workItem);

            workItem.Reassign(model.AssignedUser);

            await _uow.CompleteAsync();

            //Send work notification
            _notificationService.SendWorkNotification(workItem);

            var dto = Mapper.Map<WorkItemDto>(workItem);
            return Ok(dto);
        }

        [HttpPost, Route("submitreport/{id}")]
        public async Task<object> SubmitReport(int id)
        {
            //get work item
            var workItem = await _uow.WorkItems.GetByIdAsync(id);

            //get report
            var report = workItem.Report;

            //Retrieve file from file parameter
            foreach (string filename in HttpContext.Current.Request.Files)
            {
                var f = HttpContext.Current.Request.Files[filename];

                //Checking file is available to save.  
                if (f == null) continue;

                var reportLevel = ReportLevel.SCH;
                if (f.FileName.ToLower().Contains("SCH")) reportLevel = ReportLevel.SCH;
                if (f.FileName.ToLower().Contains("LEA")) reportLevel = ReportLevel.LEA;
                if (f.FileName.ToLower().Contains("SEA")) reportLevel = ReportLevel.SEA;

                var version = await _uow.Documents.GetNextAvailableVersion(report.SubmissionId, reportLevel);

                BinaryReader br = new BinaryReader(f.InputStream);
                byte[] data = br.ReadBytes((f.ContentLength));
                var doc = ReportDocument.Create(f.FileName, version, reportLevel, data);

                //attach report documents
                report.AddDocument(doc);
            }

            //finish work item
            workItem.Finish();

            //Start new work item

            //What's the next work
            var next = WorkItem.Next(workItem);
            if (next == WorkItemAction.Nothing)
            {
                report.SetState(next);
                report.Submission.SetState(next);
                await _uow.CompleteAsync();
                return Ok();
            }

            //Get assignee
            var groupName = report.Submission.FileSpecification.GenerationUserGroup;
            switch (next)
            {
                case WorkItemAction.Approve:
                    groupName = report.Submission.FileSpecification.ApprovalUserGroup;
                    break;
                case WorkItemAction.Submit:
                    groupName = report.Submission.FileSpecification.SubmissionUserGroup;
                    break;
            }
            var members = _membershipService.GetGroupMembers(groupName);
            if (members.IsFailure) return BadRequest(members.Error);

            var assignee = _uow.WorkItems.GetUserWithLeastAssignments(members.Value);

            var nextWorkItem = WorkItem.Create(next, assignee);

            report.SetState(next);
            report.Submission.SetState(next);

            report.AddWorkItem(nextWorkItem);

            await _uow.CompleteAsync();

            //Send work notification
            _notificationService.SendWorkNotification(nextWorkItem);

            var dto = Mapper.Map<WorkItemDto>(nextWorkItem);
            return Ok(dto);

        }

    }


    public class ErrorReportDto
    {
        public string Note { get; set; }
        //public HttpPostedFileBase[] Files { get; set; }
    }
}
