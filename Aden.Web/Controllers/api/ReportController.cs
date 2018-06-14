using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/report")]
    public class ReportController : ApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;

        public ReportController(IUnitOfWork uow, INotificationService notificationService)
        {
            _uow = uow;
            _notificationService = notificationService;
        }

        [HttpGet, Route("{datayear:int}/{filenumber}")]
        public async Task<object> Get(int datayear, string filenumber)
        {
            var reports = await _uow.Reports.GetByFileSpecificationAsync(datayear, filenumber);
            var dto = Mapper.Map<List<ReportDto>>(reports);
            return Ok(dto);
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

            //TODO: Send work notification 
            _notificationService.SendWorkNotification(workItem);

            var dto = Mapper.Map<ReportDto>(report);
            return Ok(dto);
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
