using System;
using System.Collections.Generic;

namespace Aden.Core.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? SubmissionDate { get; set; }

        public int? DataYear { get; set; }

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
    }
}