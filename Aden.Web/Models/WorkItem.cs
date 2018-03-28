using System;
using System.Collections.Generic;
using System.Linq;
using ADEN.Web.Core;
using ADEN.Web.Data;
using ALSDE.Idem;

namespace ADEN.Web.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int? ReportId { get; set; }
        public string AssignedUser { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; }

        public WorkItemAction WorkItemAction { get; set; }
        public WorkItemState WorkItemState { get; set; }

        public Report Report { get; set; }

        public bool CanCancel
        {
            get
            {
                //TODO: Cleanup
                if (WorkItemState == WorkItemState.Cancelled) return false;

                var items = Report.WorkItems.OrderBy(x => x.Id).ToList();

                var item = items.LastOrDefault();

                if (item == null) return false;

                if (item.WorkItemAction == WorkItemAction.Generate) return false;

                var p = GetPrevious(items, item);

                if (p == null) return false;

                if (p.Id == Id) return true;

                return false;
            }
        }



        private WorkItem()
        {
        }

        public void Complete()
        {
            CompletedDate = DateTime.Now;
            WorkItemState = WorkItemState.Completed;
            CreateNextWorkItem(WorkItemAction);
        }

        private void CreateNextWorkItem(WorkItemAction currentWorkItem)
        {
            WorkItem wi;

            //TODO: Refactor to factory method
            switch (currentWorkItem)
            {
                case WorkItemAction.Generate:
                    Report.GeneratedDate = DateTime.Now;
                    Report.GeneratedUser = AssignedUser;
                    Report.ReportState = ReportState.AssignedForReview;
                    Report.Submission.ReportState = ReportState.AssignedForReview;
                    wi = WorkItem.Create(WorkItemAction.Review, Report.Submission.FileSpecification.GenerationUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Review:
                    Report.ReportState = Report.Submission.ReportState = ReportState.AwaitingApproval;
                    wi = WorkItem.Create(WorkItemAction.Approve, Report.Submission.FileSpecification.ApprovalUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Approve:
                    Report.ApprovedDate = DateTime.Now;
                    Report.ApprovedUser = AssignedUser;
                    Report.ReportState = Report.Submission.ReportState = ReportState.AssignedForSubmission;
                    wi = WorkItem.Create(WorkItemAction.Submit, Report.Submission.FileSpecification.SubmissionUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.SubmitWithError:
                    Report.SubmittedDate = DateTime.Now;
                    Report.SubmittedUser = AssignedUser;
                    Report.ReportState = Report.Submission.ReportState = ReportState.CompleteWithError;
                    wi = WorkItem.Create(WorkItemAction.ReviewError, Report.Submission.FileSpecification.ApprovalUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Submit:
                    Report.SubmittedDate = DateTime.Now;
                    Report.SubmittedUser = AssignedUser;
                    Report.ReportState = Report.Submission.ReportState = ReportState.Complete;
                    break;
                case WorkItemAction.ReviewError:
                    Report.StartNewWork();
                    break;
            }
        }


        private WorkItem(WorkItemAction action, string assignee)
        {
            WorkItemAction = action;
            AssignedUser = assignee;
            AssignedDate = DateTime.Now;
            WorkItemState = WorkItemState.NotStarted;
        }

        public static WorkItem Create(WorkItemAction action, string group)
        {
            var members = GroupHelper.GetGroupMembers("CohortAdminUsers").Select(m => m.EmailAddress).ToList();

            var uow = new UnitOfWork(AdenContext.Create());
            var assignee = uow.WorkItems.GetUserWithLeastAssignments(members);

            var wi = new WorkItem(action, assignee);
            return wi;
        }

        public void Cancel()
        {
            WorkItemState = WorkItemState.Cancelled;
            CompletedDate = DateTime.Now;
        }


        private static T GetNext<T>(IEnumerable<T> list, T current)
        {
            try
            {
                return list.SkipWhile(x => !x.Equals(current)).Skip(1).First();
            }
            catch
            {
                return default(T);
            }
        }

        private static T GetPrevious<T>(IEnumerable<T> list, T current)
        {
            try
            {
                return list.TakeWhile(x => !x.Equals(current)).Last();
            }
            catch
            {
                return default(T);
            }
        }

        public void SetAction(WorkItemAction action)
        {
            WorkItemAction = action;
        }
    }
}
