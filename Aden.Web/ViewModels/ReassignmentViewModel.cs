using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aden.Web.ViewModels
{
    public class ReassignmentViewModel
    {
        public int WorkItemId { get; set; }
        public string AssignedUser { get; set; }
        public string WorkItemAction { get; set; }
    }
}
