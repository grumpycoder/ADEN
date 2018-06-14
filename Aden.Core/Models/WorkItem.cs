using System;
using System.Collections.Generic;
using System.Linq;

namespace Aden.Core.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
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

                if (Report.ReportState == ReportState.Complete || Report.ReportState == ReportState.CompleteWithError)
                    return false;

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

        private WorkItem() { }

        private WorkItem(WorkItemAction action, string assignee)
        {
            WorkItemAction = action;
            AssignedUser = assignee;
            AssignedDate = DateTime.Now;
            WorkItemState = WorkItemState.NotStarted;
        }

        public static WorkItem Create(WorkItemAction action, string assignee)
        {
            return new WorkItem(action, assignee);
        }

        public void Finish()
        {
            CompletedDate = DateTime.Now;
            WorkItemState = WorkItemState.Completed;
        }

        public static WorkItemAction Next(WorkItem workItem)
        {
            switch (workItem.WorkItemAction)
            {
                case WorkItemAction.Generate:
                    return WorkItemAction.Review;
                case WorkItemAction.Review:
                    return WorkItemAction.Approve;
                case WorkItemAction.Approve:
                    return WorkItemAction.Submit;
                case WorkItemAction.SubmitWithError:
                    return WorkItemAction.ReviewError;
                case WorkItemAction.ReviewError:
                    return WorkItemAction.Generate;
                case WorkItemAction.Submit:
                    return WorkItemAction.Nothing;
                default:
                    return WorkItemAction.Nothing;
            }

        }

        public void Cancel()
        {
            WorkItemState = WorkItemState.Cancelled;
            CompletedDate = DateTime.Now;
        }
        public void Reassign(string assignee)
        {
            AssignedUser = assignee;
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

    }
}
