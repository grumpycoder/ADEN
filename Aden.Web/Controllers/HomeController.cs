using Aden.Core.Dtos;
using Aden.Core.Repositories;
using Aden.Web.Filters;
using Aden.Web.ViewModels;
using AutoMapper;
using EAGetMail;
//using Independentsoft.Email.Mime;
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
        public async Task<ActionResult> Reports(int datayear, string filenumber)
        {
            var reports = await _uow.Reports.GetByFileSpecificationAsync(datayear, filenumber);
            //var dto = Mapper.Map<List<ReportDto>>(reports.FirstOrDefault());
            var dto = Mapper.Map<ReportDto>(reports.FirstOrDefault());
            return View(dto);
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

        [TrackViewName]
        public ActionResult Mail()
        {
            var vm = new List<MailViewModel>();
            var path = HostingEnvironment.MapPath(@"/App_Data");
            foreach (var file in Directory.GetFiles($@"{path}", "*.eml"))
            {

                var msg = new Mail("TryIt");
                msg.Load(file, false);
                vm.Add(new MailViewModel()
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Sent = msg.ReceivedDate,
                    To = msg.To.Select(s => s.Address.ToString()),
                    CC = msg.Cc.Select(s => s.Address.ToString()),

                    From = msg.From.Address,
                    Subject = msg.Subject.Replace("(Trial Version)", ""),
                    Body = msg.HtmlBody,
                    Attachments = msg.Attachments.ToList()
                });
            }

            return View(vm.OrderByDescending(x => x.Sent));
        }

    }
}