using System.ComponentModel.DataAnnotations;

namespace ADEN.Web.Models
{
    public enum ReportLevel
    {
        [Display(Name = "SEA")]
        SEA,
        [Display(Name = "LEA")]
        LEA,
        [Display(Name = "SCH")]
        SCH
    }
}