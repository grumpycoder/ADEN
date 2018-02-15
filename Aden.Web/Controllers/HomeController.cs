using System.Web.Mvc;
using ADEN.Web.Core;
using ADEN.Web.Data;
using ADEN.Web.ViewModels;

namespace Aden.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork uow;
        private string _userName = "mlawrence@alsde.edu";

        public HomeController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        public ActionResult FileSpecifications()
        {
            return View();
        }

        public ActionResult Reports(string id = null, int datayear = 0)
        {
            return View();
        }

        public ActionResult Assignments(string username)
        {
            var userName = _userName;
            if (!string.IsNullOrEmpty(username)) userName = username;
            var vm = new AssigmentsViewModel() { Username = userName };
            return View(vm);
        }

        //public ActionResult WorkItemHistory(int datayear, int id)
        public ActionResult WorkItemHistory(int reportId)
        {
            //var workItems = uow.WorkItems.GetHistoryByFileSpecification(id, datayear);
            var workItems = uow.WorkItems.GetHistoryByReport(reportId);
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
            return File(document.File, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }


    }
}