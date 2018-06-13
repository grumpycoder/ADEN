﻿using System;
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

        public virtual List<Report> Reports { get; set; }

        public byte[] SpecificationDocument { get; set; }

        public int FileSpecificationId { get; set; }
        public FileSpecification FileSpecification { get; set; }



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
                    break;
                case WorkItemAction.ReviewError:
                    SubmissionState = SubmissionState.NotStarted;
                    break;
                case WorkItemAction.Nothing:
                    SubmissionState = SubmissionState.Complete;
                    break;
            }

        }

        public void Waive()
        {
            SubmissionState = SubmissionState.Waived;
        }

    }
}