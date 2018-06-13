using Aden.Core.Helpers;
using Aden.Core.Services;
using CSharpFunctionalExtensions;
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
                    break;
                case WorkItemAction.ReviewError:
                    ReportState = ReportState.NotStarted;
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


        //REFACTOR BELOW

        //public static Result<Report> Create(Submission submission)
        //{
        //    if (submission == null) return Result.Fail<Report>("Submission should not be empty");

        //    if (string.IsNullOrWhiteSpace(submission.FileSpecification.ReportAction)) return Result.Fail<Report>("No report action defined");

        //    if (submission.DataYear == null) return Result.Fail<Report>("No data year defined on file specification.");

        //    return Result.Ok<Report>(new Report(submission.DataYear, ReportState.NotStarted));
        //}

        public void AddWorkItem(WorkItem workItem)
        {
            //TODO: Throw error is uncompleted work item exists
            WorkItems.Add(workItem);
            workItem.Report = this;
            //Send notification
            NotificationService.SendWorkNotification(workItem);
        }

        public WorkItem ReassignWorkItem(WorkItem workItem, string assignee)
        {

            //var wi = WorkItem.Create(workItem.WorkItemAction, assignee, true);
            //AddWorkItem(wi);


            //workItem.WorkItemState = WorkItemState.Reassigned;

            //NotificationService.SendReassignmentWorkNotification(workItem);
            //return wi;
            return workItem;
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

        public void CancelWorkItems()
        {
            foreach (var workItem in WorkItems.Where(i => i.WorkItemState == WorkItemState.NotStarted))
            {
                workItem.Cancel();
                //Send notification
                NotificationService.SendCancelWorkNotification(workItem);
            }

        }

        public void StartNewWork()
        {
            //var wi = WorkItem.Create(WorkItemAction.Generate, Submission.FileSpecification.GenerationUserGroup);
            //AddWorkItem(wi.Value);

            ////TODO: Does this belong here
            //ReportState = ReportState.AssignedForGeneration;
            //Submission.SubmissionState = SubmissionState.AssignedForGeneration;
        }



    }
}
