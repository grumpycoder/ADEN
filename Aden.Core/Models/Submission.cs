using System;
using System.Collections.Generic;

namespace Aden.Core.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? SubmissionDate { get; set; }

        public int DataYear { get; set; }

        public bool IsSEA { get; set; }
        public bool IsLEA { get; set; }
        public bool IsSCH { get; set; }

        public SubmissionState SubmissionState { get; set; }

        public DateTime? LastUpdated { get; set; }

        public List<Report> Reports { get; set; }

        public List<SubmissionAudit> SubmissionAudits { get; set; }

        public byte[] SpecificationDocument { get; set; }

        public int FileSpecificationId { get; set; }
        public FileSpecification FileSpecification { get; set; }

        private Submission()
        {
            Reports = new List<Report>();
            SubmissionAudits = new List<SubmissionAudit>();
        }

        private Submission(DateTime dueDate, int dataYear, bool isSea, bool isLea, bool isSch)
        {
            DueDate = dueDate;
            DataYear = dataYear;
            IsLEA = isLea;
            IsSEA = isSea;
            IsSCH = isSch;
            SubmissionState = SubmissionState.NotStarted;
        }

        public void AddReport(Report report)
        {
            report.Submission = this;
            Reports.Add(report);
        }

        public void SetState(WorkItemAction action)
        {

            switch (action)
            {
                case WorkItemAction.Generate:
                    SubmissionState = SubmissionState.AssignedForGeneration;
                    break;
                case WorkItemAction.Review:
                    SubmissionState = SubmissionState.AssignedForReview;
                    break;
                case WorkItemAction.Approve:
                    SubmissionState = SubmissionState.AwaitingApproval;
                    break;
                case WorkItemAction.Submit:
                    SubmissionState = SubmissionState.AssignedForSubmission;
                    break;
                case WorkItemAction.SubmitWithError:
                    SubmissionState = SubmissionState.CompleteWithError;
                    SubmissionDate = DateTime.Now;
                    break;
                case WorkItemAction.ReviewError:
                    SubmissionState = SubmissionState.CompleteWithError;
                    SubmissionDate = DateTime.Now;
                    break;
                case WorkItemAction.Nothing:
                    SubmissionState = SubmissionState.NotStarted;
                    SubmissionDate = null;
                    break;
            }

            LastUpdated = DateTime.Now;

        }

        public void Waive(string reason, string waivedBy)
        {
            SubmissionState = SubmissionState.Waived;
            var message = $"Waived by {waivedBy}: {reason}";
            var audit = new SubmissionAudit(Id, message);
            Log(audit);
        }

        public void Reopen(string reason, string openedBy)
        {
            var message = $"ReOpened by {openedBy}: {reason}";
            var audit = new SubmissionAudit(Id, message);
            Log(audit);
        }

        private void Log(SubmissionAudit audit)
        {
            SubmissionAudits.Add(audit);
        }


        public static Submission Create(FileSpecification fileSpecification)
        {
            return new Submission(fileSpecification.DueDate, fileSpecification.DataYear, fileSpecification.IsSEA, fileSpecification.IsLEA, fileSpecification.IsSCH);
        }

        public void Complete()
        {
            SubmissionState = SubmissionState.Complete;
            LastUpdated = DateTime.Now;
        }


        // NEW CODE
        public Report CreateReport()
        {
            var report = Report.Create(DataYear);
            Reports.Add(report);
            SubmissionState = SubmissionState.AssignedForGeneration;
            return report;
        }
    }
}