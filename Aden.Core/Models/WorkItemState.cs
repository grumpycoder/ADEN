using System.ComponentModel.DataAnnotations;

namespace Aden.Core.Models
{
    public enum WorkItemState
    {
        [Display(Name = "Not Started")]
        NotStarted = 1,
        [Display(Name = "Cancelled")]
        Cancelled = 2,
        [Display(Name = "Complete")]
        Completed = 3,
        [Display(Name = "Reassigned")]
        Reassigned = 4
    }
}