using Aden.Core.Models;
using System.Collections.Generic;

namespace Aden.Core.Dtos
{
    public class SubmissionWorkHistoryViewDto
    {
        public IList<WorkItemHistoryDto> WorkItemHistory { get; set; }
        public IList<SubmissionAudit> SubmissionAudits { get; set; }

    }
}
