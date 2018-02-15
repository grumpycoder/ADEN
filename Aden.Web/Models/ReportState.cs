using System.ComponentModel.DataAnnotations;

namespace ADEN.Web.Models
{
    public enum ReportState : byte
    {
        [Display(Name = "Not Started")]
        NotStarted = 1,
        [Display(Name = "Assigned for Generation")]
        AssignedForGeneration = 2,
        [Display(Name = "Assigned for Review")]
        AssignedForReview = 3,
        [Display(Name = "Assigned for Approval")]
        AwaitingApproval = 4,
        [Display(Name = "Assigned for Submit")]
        AssignedForSubmission = 5,
        [Display(Name = "Completed")]
        Complete = 6,
        [Display(Name = "Waived")]
        Waived = 7,
    }
}