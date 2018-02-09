using System.Collections.Generic;
using ADEN.Web.Models;

namespace ADEN.Web.ViewModels
{
    public class AssigmentsViewModel
    {
        public string Username { get; set; }
        public IEnumerable<WorkItem> WorkItems { get; set; }
        public IEnumerable<WorkItem> CompletedWorkItems { get; set; }

    }
}