using System.Collections.Generic;

namespace Aden.Web.ViewModels
{
    public class AssigmentsViewModel
    {
        public string Username { get; set; }
        public List<WorkItemViewModel> WorkItems { get; set; }
        public List<WorkItemViewModel> CompletedWorkItems { get; set; }
        public List<WorkItemViewModel> RetrievableWorkItems { get; set; }
    }
}