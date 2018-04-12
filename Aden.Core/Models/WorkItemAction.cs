﻿using System.ComponentModel.DataAnnotations;

namespace Aden.Core.Models
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
        Submit = 4,
        [Display(Name = "Submit With Error")]
        SubmitWithError = 5,
        [Display(Name = "Review Error")]
        ReviewError = 6
    }
}