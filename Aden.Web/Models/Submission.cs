using System;
using System.Collections.Generic;

namespace ADEN.Web.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DataYear { get; set; }

        public bool IsSEA { get; set; }
        public bool IsLEA { get; set; }
        public bool IsSCH { get; set; }

        public ReportState ReportState { get; set; }

        public virtual List<Report> Reports { get; set; }

        public byte[] SpecificationDocument { get; set; }

        public int FileSpecificationId { get; set; }
        public virtual FileSpecification FileSpecification { get; set; }

        public void AddReport(Report report)
        {
            report.Submission = this;
            Reports.Add(report);
        }
    }
}