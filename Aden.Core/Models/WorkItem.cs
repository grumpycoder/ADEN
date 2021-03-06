﻿using System;
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
        public string Description { get; set; }

        public List<WorkItemImage> WorkItemImages { get; set; }

        public WorkItemAction WorkItemAction { get; set; }
        public WorkItemState WorkItemState { get; set; }

        public Report Report { get; set; }

        public bool CanCancel
        {
            get
            {
                return true;
                //TODO: Cleanup
                //if (WorkItemState == WorkItemState.Cancelled) return false;

                //if (Report.ReportState == ReportState.Complete || Report.ReportState == ReportState.CompleteWithError)
                //    return false;

                //var items = Report.WorkItems.Where(x => x.Report.ReportState != ReportState.Complete || x.Report.ReportState != ReportState.CompleteWithError).OrderBy(x => x.Id).ToList();

                ////var items = Report.WorkItems.OrderBy(x => x.Id).ToList();
                //var item = items.LastOrDefault();
                //if (item == null) return false;

                //if (item.WorkItemAction == WorkItemAction.Generate) return false;

                //var p = GetPrevious(items, item);

                //if (p == null) return false;

                //return p.Id == Id;
            }
        }

        private WorkItem()
        {
            
        }

        private WorkItem(WorkItemAction action, string assignee): base()
        {
            WorkItemAction = action;
            AssignedUser = assignee;
            AssignedDate = DateTime.Now;
            WorkItemState = WorkItemState.NotStarted;
            WorkItemImages = new List<WorkItemImage>();
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
            if (workItem.WorkItemState == WorkItemState.Reject) return WorkItemAction.Generate;

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
                case WorkItemAction.Reject:
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

        public void Reject()
        {
            WorkItemState = WorkItemState.Reject;
            CompletedDate = DateTime.Now;
        }

        public void AddImages(List<byte[]> files)
        {
            foreach (var file in files)
            {
                var workItemImage = new WorkItemImage();
                workItemImage.Image = file;
                WorkItemImages.Add(workItemImage);
            }
        }

    }
}
