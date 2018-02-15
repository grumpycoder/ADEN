using System.Collections.Generic;
using System.Web.Mvc;
using ADEN.Web.Core;
using ADEN.Web.Data;
using ADEN.Web.ViewModels;
using AutoMapper;

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
            _userName = username;
            var vm = new AssigmentsViewModel();
            var workitems = uow.WorkItems.GetActiveByUser(_userName);
            var completedWorkItems = uow.WorkItems.GetCompletedByUser(username);

            var wi = Mapper.Map<List<WorkItemViewModel>>(workitems);
            var wi2 = Mapper.Map<List<WorkItemViewModel>>(completedWorkItems);

            vm.WorkItems = wi;
            vm.CompletedWorkItems = wi2;
            vm.Username = username;
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
            return File(document.File, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }


    }
}