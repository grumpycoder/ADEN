using System.ComponentModel.DataAnnotations;

namespace Aden.Core.Models
{
    public enum ReportState : byte
    {
        [Display(Name = "Not Started")]
        NotStarted = 1,
        [Display(Name = "Assigned for Generate")]
        AssignedForGeneration = 2,
        [Display(Name = "Assigned for Review")]
        AssignedForReview = 3,
        [Display(Name = "Assigned for Approve")]
        AwaitingApproval = 4,
        [Display(Name = "Assigned for Submit")]
        AssignedForSubmission = 5,
        [Display(Name = "Complete with Errors")]
        CompleteWithError = 6,
        [Display(Name = "Completed")]
        Complete = 7,
        [Display(Name = "Waived")]
        Waived = 8,
    }
}