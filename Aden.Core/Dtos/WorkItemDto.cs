using Aden.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aden.Core.Dtos
{
    public class WorkItemDto
    {

        public int Id { get; set; }
        public int ReportId { get; set; }
        public int DataYear { get; set; }

        [Display(Name = "Data Year")]
        public string DisplayDataYear => $"{DataYear - 1}-{DataYear}";

        public DateTime? AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string State { get; set; }
        public string Action { get; set; }
        public string ReportAction { get; set; }

        public WorkItemAction WorkItemActionId { get; set; }

        public string FileName { get; set; }
        public string FileNumber { get; set; }

        public bool CanCancel { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool IsManualUpload => ReportAction.ToLower() == "manual";


        [Required]
        public string Description { get; set; }

    }
}
