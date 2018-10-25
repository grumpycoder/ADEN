namespace Aden.Core.Dtos
{
    public class ReassignmentDto
    {
        public int WorkItemId { get; set; }
        public string AssignedUser { get; set; }
        public string WorkItemAction { get; set; }

        public string Reason { get; set; }
    }
}
