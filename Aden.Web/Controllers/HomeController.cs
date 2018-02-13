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
            //TODO: Not needed if webapi populating in view
            //var specs = uow.FileSpecifications.GetAllWithReports();
            //return View(specs);
            return View();
        }

        public ActionResult Reports(string id = null, int datayear = 0)
        {
            //TODO: Not needed if webapi populating in view
            var reports = uow.Reports.GetByFileSpecificationNumber(id, datayear);
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

        public ActionResult WorkItemHistory(int datayear, int id)
        {
            var workItems = uow.WorkItems.GetHistoryByFileSpecification(id, datayear);
            return PartialView("_WorkItemHistory", workItems);
        }

        public ActionResult Document(int id)
        {
            var document = uow.Documents.GetById(id);
            return PartialView(document);
        }

        public FileResult Download(int id)
        {
            var document = uow.Documents.GetById(id);
            //byte[] byteArray = Convert.FromBase64String(document.File);
            return File(document.File, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }


    }
}