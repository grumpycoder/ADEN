using System;
using System.Collections.Generic;
using System.Linq;

namespace ADEN.Web.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int? ReportId { get; set; }
        public string AssignedUser { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

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
                    Report.FileSpecification.ReportState = ReportState.AssignedForReview;
                    wi = WorkItem.Create(WorkItemAction.Review, Report.FileSpecification.GenerationUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Review:
                    Report.ReportState = Report.FileSpecification.ReportState = ReportState.AwaitingApproval;
                    wi = WorkItem.Create(WorkItemAction.Approve, Report.FileSpecification.ApprovalUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Approve:
                    Report.ApprovedDate = DateTime.Now;
                    Report.ApprovedUser = AssignedUser;
                    Report.ReportState = Report.FileSpecification.ReportState = ReportState.AssignedForSubmission;
                    wi = WorkItem.Create(WorkItemAction.Submit, Report.FileSpecification.SubmissionUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Submit:
                    Report.SubmittedDate = DateTime.Now;
                    Report.SubmittedUser = AssignedUser;
                    Report.ReportState = Report.FileSpecification.ReportState = ReportState.Complete;
                    break;
                case WorkItemAction.SubmitWithError:
                    Report.SubmittedDate = DateTime.Now;
                    Report.SubmittedUser = AssignedUser;
                    Report.ReportState = Report.FileSpecification.ReportState = ReportState.CompleteWithError;
                    break;
            }
        }

        private WorkItem()
        {
        }

        private WorkItem(WorkItemAction action, string assignee)
        {
            WorkItemAction = action;
            AssignedUser = assignee;
            AssignedDate = DateTime.Now;
            WorkItemState = WorkItemState.NotStarted;
        }

        public static WorkItem Create(WorkItemAction action, string assignee)
        {
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
