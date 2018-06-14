using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Web.Filters;
using AutoMapper;
using System.Collections.Generic;
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
            //TODO: refactor
            var isAdministrator = ((ClaimsPrincipal)User).Claims.Where(c => c.Value.ToLower().Contains("administrator")).Count() > 0;

            ViewBag.IsSectionAdmin = isAdministrator;
            var workItems = await _uow.WorkItems.GetHistoryAsync(reportId);

            var list = Mapper.Map<List<WorkItemHistoryDto>>(workItems);

            return PartialView("_WorkHistory", list);
        }

        public async Task<ActionResult> Reassign(int workItemId)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(workItemId);
            //TODO: Refactor to Automapper
            var dto = new ReassignmentDto()
            {
                WorkItemId = workItem.Id,
                AssignedUser = workItem.AssignedUser,
                WorkItemAction = workItem.WorkItemAction.ToString()
            };

            return PartialView("_WorkItemReassignment", dto);
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

            var model = Mapper.Map<UpdateFileSpecificationDto>(spec);
            return PartialView("_FileSpecificationForm", model);

        }

        public async Task<ActionResult> ErrorReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<WorkItemDto>(wi);
            return PartialView("_WorkItemForm", model);
        }

        public async Task<ActionResult> UploadReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<ReportUploadDto>(wi);
            return PartialView("_ReportUploadForm", model);
        }

        [HttpPost]
        public async Task<ActionResult> UploadReport(ReportUploadDto model, HttpPostedFileBase[] files)
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