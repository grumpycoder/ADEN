using Aden.Core.Helpers;
using Aden.Core.Models;
using System;

namespace Aden.Core.Dtos
{
    public class WorkItemHistoryDto
    {
        public int Id { get; set; }
        public WorkItemAction Action { get; set; }
        public string ActionDescription => Action.GetDisplayName();
        public string Status => WorkItemState.GetDisplayName();
        public DateTime? AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string AssignedUser { get; set; }
        public WorkItemState WorkItemState { get; set; }
        public string Description { get; set; }


    }
}
