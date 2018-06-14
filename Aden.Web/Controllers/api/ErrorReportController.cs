
using Aden.Core.Models;
using Aden.Core.Repositories;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aden.Web.Controllers.api
{
    public class ErrorReportController : Controller
    {
        // GET: ErrorReport
        private readonly IUnitOfWork _uow;

        public ErrorReportController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost, Route("assignment/submiterror")]
        public async Task<object> ErrorReport(SubmissionErrorDto model)
        {
            //if (!ModelState.IsValid) return PartialView("_WorkItemForm", model);

            var workItem = await _uow.WorkItems.GetByIdAsync(model.Id);
            workItem.Notes = model.Note;
            workItem.Finish();

            var report = await _uow.Reports.GetByIdAsync(workItem.ReportId);

            var nextWorkItem = WorkItem.Create(WorkItemAction.ReviewError, "mlawrence@alsde.edu");

            report.SetState(nextWorkItem.WorkItemAction);
            report.Submission.SetState(nextWorkItem.WorkItemAction);

            report.WorkItems.Add(nextWorkItem);

            //Send email notifications

            await _uow.CompleteAsync();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }

    public class SubmissionErrorDto
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
    }
}