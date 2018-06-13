using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using AutoMapper;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/report")]
    public class ReportController : ApiController
    {
        private readonly IUnitOfWork _uow;

        public ReportController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost, Route("create/{id}")]
        public async Task<object> Create(int id)
        {
            var submission = await _uow.Submissions.GetByIdAsync(id);

            if (submission == null) return NotFound();

            var report = Report.Create(submission.DataYear);

            submission.Reports.Add(report);
            //TODO: Find assignee from group
            var workItem = WorkItem.Create(WorkItemAction.Generate, "mlawrence@alsde.edu");

            report.WorkItems.Add(workItem);
            report.SetState(workItem.WorkItemAction);
            submission.SetState(workItem.WorkItemAction);

            await _uow.CompleteAsync();

            return Ok(report);
        }

        [HttpPost, Route("waiver/{id}")]
        public async Task<object> Waiver(int id)
        {
            var submission = await _uow.Submissions.GetByIdAsync(id);
            if (submission == null) return NotFound();

            var report = Report.Create(submission.DataYear);

            submission.Reports.Add(report);

            submission.Waive();
            report.Waive();

            await _uow.CompleteAsync();

            var dto = Mapper.Map<ReportDto>(report);

            return Ok(dto);
        }


    }
}
