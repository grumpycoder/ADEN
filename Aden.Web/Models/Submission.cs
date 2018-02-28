using System;
using System.Collections.Generic;

namespace ADEN.Web.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DataYear { get; set; }

        public ReportState ReportState { get; set; }
        public string ReportAction { get; set; }

        public virtual List<Report> Reports { get; set; }

        public byte[] SpecificationDocument { get; set; }

        public int FileSpecificationId { get; set; }
        public virtual FileSpecification FileSpecification { get; set; }


    }
}