using Aden.Core.Dtos;
using Aden.Core.Repositories;
using Aden.Web.Filters;
using Aden.Web.ViewModels;
using AutoMapper;
using Independentsoft.Email.Mime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;


namespace Aden.Web.Controllers
{
    //[Authorize]
    [CustomAuthorize(Roles = "AdenAppUsers")]
    public class HomeController : AsyncController
    {
        private readonly IUnitOfWork _uow;

        public HomeController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        [TrackViewName]
        [CustomAuthorize(Roles = Constants.GlobalAdministratorGroup)]
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
            var isAdministrator = ((ClaimsPrincipal)User).Claims.Any(c => c.Value.ToLower().Contains("administrator"));

            ViewBag.IsSectionAdmin = isAdministrator;
            var workItems = await _uow.WorkItems.GetHistoryAsync(reportId);

            var list = Mapper.Map<List<WorkItemHistoryDto>>(workItems);

            return PartialView("_WorkHistory", list);
        }

        public async Task<ActionResult> Reassign(int workItemId)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(workItemId);

            var dto = Mapper.Map<ReassignmentDto>(workItem);

            return PartialView("_WorkItemReassignment", dto);
        }

        public async Task<ActionResult> Document(int id)
        {
            var document = await _uow.Documents.GetByIdAsync(id);

            var dto = Mapper.Map<ReportDocumentDto>(document);

            return PartialView(dto);
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
            return PartialView("_ErrorReportForm", model);
        }

        public async Task<ActionResult> UploadReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<ReportUploadDto>(wi);
            return PartialView("_ReportUploadForm", model);
        }

        public ActionResult Error(string message)
        {
            return View();
        }

        public ActionResult Mail()
        {
            var vm = new List<MailViewModel>();
            var path = HostingEnvironment.MapPath(@"/App_Data");
            foreach (var file in Directory.GetFiles($@"{path}", "*.eml"))
            {
                var message = new Message(file);


                var body = message.BodyParts.Count > 1 ? message.BodyParts.FirstOrDefault()?.Body : message.Body;
                var attachmentList = message.GetAttachments().ToList();

                vm.Add(new MailViewModel()
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Sent = message.Date,
                    To = message.To.Select(s => s.EmailAddress.ToString()),
                    CC = message.Cc.Select(s => s.EmailAddress.ToString()),
                    From = message.From.EmailAddress,
                    Subject = message.Subject,
                    Body = body,
                    Attachments = attachmentList
                });
            }

            return View(vm.OrderByDescending(x => x.Sent));
        }

    }
}