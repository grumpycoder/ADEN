using System.ComponentModel.DataAnnotations;

namespace ADEN.Web.Models
{
    public enum WorkItemAction
    {
        [Display(Name = "Generate")]
        Generate = 1,
        [Display(Name = "Review")]
        Review = 2,
        [Display(Name = "Approve")]
        Approve = 3,
        [Display(Name = "Submit")]
        Submit = 4
    }
}