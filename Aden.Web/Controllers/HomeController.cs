using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using Aden.Web.Filters;
using Aden.Web.ViewModels;
using AutoMapper;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aden.Web.Controllers
{
    [Authorize]
    public class HomeController : AsyncController
    {
        private readonly IUnitOfWork _uow;

        public HomeController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        //TODO: Remove hardcoded roles, these will change 
        [TrackViewName]
        [CustomAuthorize(Roles = "MarkAdenAppGlobalAdministrators")]
        public ActionResult FileSpecifications()
        {
            return View();
        }

        [TrackViewName]
        public ActionResult Submissions()
        {
            return View();
        }

        [TrackViewName]
        public ActionResult Reports(string id = null, int datayear = 0)
        {
            return View();
        }

        [TrackViewName]
        public ActionResult Assignments(string view, string username)
        {
            return View((object)User.Identity.Name);
        }

        public async Task<ActionResult> WorkHistory(int reportId)
        {
            var isAdministrator = ((ClaimsPrincipal)User).Claims.Where(c => c.Value.ToLower().Contains("administrator")).Count() > 0;

            ViewBag.IsSectionAdmin = isAdministrator;
            var workItems = await _uow.WorkItems.GetHistoryAsync(reportId);
            return PartialView("_WorkHistory", workItems);
        }

        public async Task<ActionResult> Reassign(int workItemId)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(workItemId);
            var model = new ReassignmentViewModel()
            {
                WorkItemId = workItem.Id,
                AssignedUser = workItem.AssignedUser,
                WorkItemAction = workItem.WorkItemAction.ToString()
            };

            return PartialView("_WorkItemReassignment", model);
        }

        public async Task<ActionResult> Document(int id)
        {
            var document = await _uow.Documents.GetByIdAsync(id);
            return PartialView(document);
        }

        public async Task<FileResult> Download(int id)
        {
            var document = await _uow.Documents.GetByIdAsync(id);
            return File(document.FileData, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }

        public ActionResult EditFileSpecification(int id)
        {
            var spec = _uow.FileSpecifications.GetById(id);

            var model = Mapper.Map<FileSpecificationDto>(spec);
            return PartialView("_FileSpecificationForm", model);

        }

        [HttpPost]
        public async Task<ActionResult> Update(FileSpecificationDto model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_FileSpecificationForm", model);
            }
            var spec = await _uow.FileSpecifications.GetByIdAsync(model.Id);

            Mapper.Map(model, spec);
            await _uow.CompleteAsync();

            return Content("success");

        }

        public async Task<ActionResult> UploadErrorReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<WorkItemViewModel>(wi);
            return PartialView("_WorkItemForm", model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveErrorReport(WorkItemViewModel model, HttpPostedFileBase[] files)
        {
            //TODO: Cleanup SaveWorkItem method
            if (!ModelState.IsValid)
            {
                return PartialView("_WorkItemForm", model);
            }
            //TODO: Refactor this to webapi controller
            var wi = await _uow.WorkItems.GetByIdAsync(model.Id);
            wi.Notes = model.Notes;
            wi.SetAction(WorkItemAction.SubmitWithError);

            wi.Finish();
            await _uow.CompleteAsync();

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

        public async Task<ActionResult> UploadReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<ReportUploadViewModel>(wi);
            return PartialView("_ReportUploadForm", model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveReport(ReportUploadViewModel model, HttpPostedFileBase[] files)
        {
            //TODO: Cleanup SaveReport method
            if (!ModelState.IsValid)
            {
                return PartialView("_ReportUploadForm", model);
            }
            //TODO: Refactor this to webapi controller
            var wi = await _uow.WorkItems.GetByIdAsync(model.Id);

            foreach (var f in files)
            {
                //Checking file is available to save.  
                if (f == null) continue;
                var reportLevel = ReportLevel.SCH;
                if (f.FileName.ToLower().Contains("SCH")) reportLevel = ReportLevel.SCH;
                if (f.FileName.ToLower().Contains("LEA")) reportLevel = ReportLevel.LEA;
                if (f.FileName.ToLower().Contains("SEA")) reportLevel = ReportLevel.SEA;

                var version = await _uow.Documents.GetNextAvailableVersion(wi.Report.SubmissionId, reportLevel);

                BinaryReader br = new BinaryReader(f.InputStream);
                byte[] data = br.ReadBytes((f.ContentLength));
                var doc = ReportDocument.Create(f.FileName, version, reportLevel, data);
                wi.Report.Documents.Add(doc);

            }

            wi.Finish();
            await _uow.CompleteAsync();

            return Content("success");
        }

        public ActionResult Error(string message)
        {
            return View();
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

    }
}