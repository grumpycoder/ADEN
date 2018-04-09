using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aden.Core.Data;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using Aden.Web.Filters;
using Aden.Web.ViewModels;
using AutoMapper;

namespace Aden.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UnitOfWork uow;
        private string _userName = "mlawrence@alsde.edu";

        public HomeController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        [TrackViewName]
        public ActionResult FileSpecifications(string view)
        {
            var viewName = view == "x" ? "FileSpecificationsX" : "FileSpecifications";
            return View(viewName);
        }

        [TrackViewName]
        public ActionResult Submissions(string view)
        {
            var user = HttpContext.User.Identity;

            ViewBag.Message = "You are logged in: " + user.Name;

            var viewName = view == "x" ? "SubmissionsX" : "Submissions";
            return View(viewName);
        }

        [TrackViewName]
        public ActionResult Reports(string view, string id = null, int datayear = 0)
        {
            var user = HttpContext.User.Identity;

            ViewBag.Message = "You are logged in: " + user.Name;

            var viewName = view == "x" ? "ReportsX" : "Reports";

            return View(viewName);
        }

        [TrackViewName]
        public ActionResult Assignments(string view, string username)
        {
            var user = HttpContext.User.Identity;

            var userName = _userName;

            var viewName = view == "x" ? "AssignmentsX" : "Assignments";

            if (!string.IsNullOrEmpty(username)) userName = username;
            var vm = new AssigmentsViewModel() { Username = user.Name };
            return View(viewName, vm);
        }

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
            return File(document.FileData, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }

        public ActionResult EditFileSpecification(int id)
        {
            var spec = uow.FileSpecifications.GetById(id);

            var model = Mapper.Map<FileSpecificationEditViewModel>(spec);
            return PartialView("_FileSpecificationForm", model);

        }

        [HttpPost]
        public ActionResult SaveSpecification(FileSpecificationEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_FileSpecificationForm", model);
            }
            var spec = uow.FileSpecifications.GetById(model.Id);

            Mapper.Map(model, spec);
            uow.Complete();

            return Content("success");

        }

        public ActionResult EditWorkItem(int id)
        {
            var wi = uow.WorkItems.GetById(id);

            var model = Mapper.Map<WorkItemViewModel>(wi);
            return PartialView("_WorkItemForm", model);
        }

        [HttpPost]
        public ActionResult SaveWorkItem(WorkItemViewModel model, HttpPostedFileBase[] files)
        {

            var wi = uow.WorkItems.GetById(model.Id);
            wi.Notes = model.Notes;
            wi.SetAction(WorkItemAction.SubmitWithError);

            //wi.Complete();
            //uow.Complete();

            var next = wi.Report.WorkItems.LastOrDefault();

            foreach (var f in files)
            {
                //Checking file is available to save.  
                if (f == null) continue;
                var inputFileName = Path.GetFileName(f.FileName);
                var serverSavePath = Path.Combine(Server.MapPath("~/App_Data/") + inputFileName);
                f.SaveAs(serverSavePath);
                //assigning file uploaded status to ViewBag for showing message to user.  
                ViewBag.UploadStatus = files.Count() + " files uploaded successfully.";
            }

            NotificationService.SendWorkItemError(next, wi.Notes, Server.MapPath("~/App_Data/"));

            var path = Server.MapPath("~/App_Data/");

            foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
            {
                file.Delete();
            }

            return Content("success");
        }

        public ActionResult Error(string message)
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult SaveWorkItem(WorkItemViewModel model, HttpPostedFileBase files)
        //{
        //    var wi = uow.WorkItems.GetById(model.Id);

        //    wi.Notes = model.Notes;

        //    var file = files;
        //    var InputFileName = Path.GetFileName(file.FileName);
        //    var ServerSavePath = Path.Combine(Server.MapPath("~/App_Data/") + InputFileName);
        //    //Save file to server folder  
        //    file.SaveAs(ServerSavePath);
        //    return Content("success");
        //}
    }
}