using Aden.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aden.Core.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int? DataYear { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string GeneratedUser { get; set; }
        public string ApprovedUser { get; set; }
        public string SubmittedUser { get; set; }

        public List<ReportDocument> Documents { get; set; }

        public int SubmissionId { get; set; }
        public Submission Submission { get; set; }

        public ReportState ReportState { set; get; }

        public List<WorkItem> WorkItems { set; get; }

        private Report()
        {
            WorkItems = new List<WorkItem>();
            Documents = new List<ReportDocument>();
        }

        private Report(int? dataYear) : this()
        {
            DataYear = dataYear;
            ReportState = ReportState.NotStarted;
        }

        public static Report Create(int datayear)
        {
            var report = new Report(datayear);
            return report;
        }

        public void SetState(WorkItemAction action)
        {
            switch (action)
            {
                case WorkItemAction.Generate:
                    ReportState = ReportState.AssignedForGeneration;
                    break;
                case WorkItemAction.Review:
                    ReportState = ReportState.AssignedForReview;
                    GeneratedDate = DateTime.Now;
                    break;
                case WorkItemAction.Approve:
                    ReportState = ReportState.AwaitingApproval;
                    break;
                case WorkItemAction.Submit:
                    ReportState = ReportState.AssignedForSubmission;
                    ApprovedDate = DateTime.Now;
                    break;
                case WorkItemAction.SubmitWithError:
                    ReportState = ReportState.CompleteWithError;
                    SubmittedDate = DateTime.Now;
                    break;
                case WorkItemAction.ReviewError:
                    ReportState = ReportState.CompleteWithError;
                    SubmittedDate = DateTime.Now;
                    break;
                case WorkItemAction.Nothing:
                    ReportState = ReportState.Complete;
                    SubmittedDate = DateTime.Now;
                    break;
            }
        }

        public void Waive()
        {
            ReportState = ReportState.Waived;
        }

        public void CreateDocument(byte[] file, ReportLevel reportLevel)
        {
            var version = 0;
            if (Documents.Any(d => d.ReportLevel == reportLevel)) version = Documents.Max(x => x.Version);

            version += 1;
            var filename = Submission.FileSpecification.FileNameFormat.Replace("{level}", reportLevel.GetDisplayName()).Replace("{version}", string.Format("v{0}.csv", version));

            var doc = ReportDocument.Create(filename, version, reportLevel, file);
            Documents.Add(doc);
        }

        public void AddWorkItem(WorkItem workItem)
        {
            WorkItems.Add(workItem);
        }

        public void AddDocument(ReportDocument document)
        {
            Documents.Add(document);
        }

        public void CancelWorkItems()
        {
            WorkItems.RemoveAll(x => x.WorkItemState == WorkItemState.NotStarted);

            //foreach (var workItem in WorkItems.Where(i => i.WorkItemState == WorkItemState.NotStarted))
            //{
            //    workItem.Cancel();
            //    //Send notification
            //}
        }

        public void DeleteWorkItems()
        {
            WorkItems.RemoveAll(x => x.ReportId == Id);

            //foreach (var workItem in WorkItems.Where(i => i.WorkItemState == WorkItemState.NotStarted))
            //{
            //    workItem.Cancel();
            //    //Send notification
            //}
        }
    }
}
