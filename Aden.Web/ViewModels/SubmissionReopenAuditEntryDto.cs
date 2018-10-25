using System;

namespace Aden.Web.ViewModels
{
    public class SubmissionReopenAuditEntryDto
    {
        public int SubmissionId { get; set; }
        public string Message { get; set; }

        public DateTime NextSubmissionDate { get; set; }
    }
}