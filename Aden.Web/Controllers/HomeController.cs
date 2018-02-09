using ADEN.Web.Core;
using ADEN.Web.Data;
using ADEN.Web.ViewModels;
using System.Web.Mvc;

namespace Aden.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork uow;
        private string UserName = "mlawrence@alsde.edu";

        public HomeController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        public ActionResult FileSpecifications()
        {
            var specs = uow.FileSpecifications.GetAllWithReports();
            return View(specs);
        }

        public ActionResult Reports(string id)
        {
            var reports = uow.Reports.GetByFileSpecificationNumber(id);
            return View(reports);
        }

        public ActionResult Assignments(string username)
        {
            UserName = username;
            var vm = new AssigmentsViewModel();
            var workitems = uow.WorkItems.GetActiveByUser(UserName);
            var completedWorkItems = uow.WorkItems.GetCompletedByUser(username);
            vm.WorkItems = workitems;
            vm.CompletedWorkItems = completedWorkItems;

            return View(vm);
        }
    }
}