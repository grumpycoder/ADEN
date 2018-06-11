using System.Collections.Generic;

namespace Aden.Core.Dtos
{
    public class AssignmentDto
    {
        public string Username { get; set; }
        public List<WorkItemDto> WorkItems { get; set; }
        public List<WorkItemDto> CompletedWorkItems { get; set; }
        public List<WorkItemDto> RetrievableWorkItems { get; set; }
    }
}
