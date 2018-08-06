using Aden.Core.Models;
using System;
using System.Collections.Generic;

namespace Aden.Core.Dtos
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }
        public int DataYear { get; set; }

        public string DisplayDataYear => $"{DataYear - 1}-{DataYear}";

        public ReportState ReportStateId { get; set; }
        public string ReportState { get; set; }
        public int FileSpecificationId { get; set; }
        public List<ReportDocumentDto> Documents { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }

    }
}
