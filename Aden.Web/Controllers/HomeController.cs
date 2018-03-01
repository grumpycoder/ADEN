using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aden.Web.Services;
using ADEN.Web.Core;
using ADEN.Web.Data;
using ADEN.Web.Models;
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
            var userName = _userName;
            if (!string.IsNullOrEmpty(username)) userName = username;
            var vm = new AssigmentsViewModel() { Username = userName };
            return View(vm);
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
                return View("_FileSpecificationForm", model);
            }

            var spec = uow.FileSpecifications.GetById(model.Id);
            //TODO: Use Automapper
            spec.FileName = model.FileName;
            spec.FileNumber = model.FileNumber;
            spec.Department = model.Department;
            spec.GenerationUserGroup = model.Department;
            spec.ApprovalUserGroup = model.Department;
            spec.SubmissionUserGroup = model.Department;
            spec.FileNameFormat = model.Department;
            spec.IsRetired = model.IsRetired;
            spec.ReportAction = model.ReportAction;

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

            wi.Complete();
            uow.Complete();

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