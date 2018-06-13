using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/assignment")]
    public class AssignmentController : ApiController
    {
        private readonly IUnitOfWork _uow;

        public AssignmentController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            var id = "mlawrence@alsde.edu"; // User.Identity.Name ?? id;

            var workitems = await _uow.WorkItems.GetActiveAsync(id);

            var wi = Mapper.Map<List<WorkItemDto>>(workitems);

            return Ok(wi);
        }

        [HttpPost, Route("complete/{id}")]
        public async Task<object> Complete(int id)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(id);

            if (workItem == null) return NotFound();

            //TODO: Generate documents
            workItem.Finish();

            //Start new work item
            var report = await _uow.Reports.GetByIdAsync(workItem.ReportId);

            //What's the next work
            var next = WorkItem.Next(workItem);
            if (next == WorkItemAction.Nothing)
            {
                report.SetState(next);
                report.Submission.SetState(next);
                await _uow.CompleteAsync();
                return Ok();
            }

            //TODO: Get assignee
            var nextWorkItem = WorkItem.Create(next, "mlawrence@alsde.edu");


            //TODO: Set ReportState and SubmissionState
            report.SetState(next);
            report.Submission.SetState(next);

            report.WorkItems.Add(nextWorkItem);

            await _uow.CompleteAsync();

            return Ok(nextWorkItem);
        }

        [HttpPost, Route("cancel/{id}")]
        public async Task<object> Cancel(int id)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(id);

            if (workItem == null) return NotFound();

            var report = await _uow.Reports.GetByIdAsync(workItem.ReportId);

            workItem.Cancel();

            //TODO: Set ReportState and SubmissionState
            report.SetState(WorkItemAction.Nothing);
            report.Submission.SetState(WorkItemAction.Nothing);

            var newWorkItem = WorkItem.Create(WorkItemAction.Generate, "mlawrence@alsde.edu");

            report.WorkItems.Add(newWorkItem);
            report.SetState(newWorkItem.WorkItemAction);
            report.Submission.SetState(newWorkItem.WorkItemAction);

            await _uow.CompleteAsync();

            return Ok(newWorkItem);
        }
    }
}
