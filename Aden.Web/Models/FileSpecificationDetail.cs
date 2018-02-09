using System;
using System.Collections.Generic;

namespace ADEN.Web.Models
{
    public class FileSpecificationDetail
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DataYear { get; set; }
        public string FileNameFormat { get; set; }

        public int FileSpecificationId { get; set; }
        public virtual FileSpecification FileSpecification { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

        public void AddReport(Report report)
        {
            Reports.Add(report);
        }
    }
}