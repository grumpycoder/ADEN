using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using Aden.Web.Helpers;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Aden.Web.Controllers.api
{
    public class ErrorReportController : Controller
    {
        // GET: ErrorReport
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;

        public ErrorReportController(IUnitOfWork uow, INotificationService notificationService, IMembershipService membershipService)
        {
            _uow = uow;
            _notificationService = notificationService;
            _membershipService = membershipService;
        }

        [HttpPost, Route("assignment/submiterror")]
        public object ErrorReport(SubmissionErrorDto model)
        {
            //if (!ModelState.IsValid) return PartialView("_WorkItemForm", model);

            var workItem = _uow.WorkItems.GetById(model.Id);
            workItem.Description = model.Description;
            workItem.Finish();

            _notificationService.SendWorkErrorNotification(workItem, model.Files);

            var report = _uow.Reports.GetById(workItem.ReportId);

            //Get assignee
            var members = _membershipService.GetGroupMembers(report.Submission.FileSpecification.GenerationUserGroup);
            if (members.IsFailure) return new HttpStatusCodeResult(HttpStatusCode.BadRequest, members.Error);

            var assignee = _uow.WorkItems.GetUserWithLeastAssignments(members.Value);

            var nextWorkItem = WorkItem.Create(WorkItemAction.ReviewError, assignee);

            report.SetState(nextWorkItem.WorkItemAction);
            report.Submission.SetState(nextWorkItem.WorkItemAction);

            List<byte[]> files = new List<byte[]>();
            //TODO: Should not need to check for null
            if (model.Files != null)
            {
                foreach (var f in model.Files)
                {
                    files.Add(f.ConvertToByte());
                    //files.Add(ConvertToByte(f));
                }
            }

            nextWorkItem.Description = model.Description;
            nextWorkItem.AddImages(files);
            report.AddWorkItem(nextWorkItem);

            _uow.Complete();

            //Send email notifications
            _notificationService.SendWorkNotification(nextWorkItem);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


    }

    public class SubmissionErrorDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
    }


}