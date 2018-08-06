using Aden.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Aden.Core.Dtos
{
    public class SubmissionDto
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string Section { get; set; }
        public string DataGroups { get; set; }
        public string Application { get; set; }
        public string Collection { get; set; }
        public string DataSource { get; set; }

        public int? DataYear { get; set; }

        public string DisplayDataYear => string.Format("{0}-{1}", DataYear - 1, DataYear);

        public bool IsSEA { get; set; }
        public bool IsLEA { get; set; }
        public bool IsSCH { get; set; }

        public string FileNumber { get; set; }
        public string FileName { get; set; }

        public string SubmissionState { get; set; }
        public string SubmissionStateKey { get; set; }
        public SubmissionState SubmissionStateId { get; set; }

        public int? MostRecentReportId { get; set; }

        public virtual List<Report> Reports { get; set; }

        public byte[] SpecificationDocument { get; set; }

        public int FileSpecificationId { get; set; }
        public virtual FileSpecification FileSpecification { get; set; }

        public bool CanStartReport => !string.IsNullOrEmpty(FileSpecification.ReportAction);

        public bool HasAdmin
        {
            get
            {
                var claim = (HttpContext.Current.User as ClaimsPrincipal).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value.Contains("MarkAdenAppAdministrators"));
                return claim != null;
            }
        }

    }
}
