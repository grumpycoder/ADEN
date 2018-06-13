using Aden.Core.Dtos;
using Aden.Core.Repositories;
using Aden.Core.Services;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMembershipService _service;

        public ReportsController(IUnitOfWork uow, IMembershipService service)
        {
            _uow = uow;
            _service = service;
        }

        [HttpGet, Route("{datayear:int}")]
        public async Task<object> Get(int datayear)
        {
            var reports = await _uow.Reports.GetByFileSpecificationAsync(datayear);
            var reportList = Mapper.Map<List<ReportDto>>(reports);
            return Ok(reportList);
        }

        [HttpGet, Route("{datayear:int}/{filenumber}")]
        public async Task<object> Get(int datayear, string filenumber)
        {
            var reports = await _uow.Reports.GetByFileSpecificationAsync(datayear, filenumber);
            var reportList = Mapper.Map<List<ReportDto>>(reports);
            return Ok(reportList);
        }

        [HttpPost, Route("create/{submissionid}")]
        public async Task<object> Create(int submissionid)
        {
            var submission = _uow.Submissions.GetById(submissionid);
            if (submission == null) return NotFound();

            //var reportOrError = Report.Create(submission);
            //if (reportOrError.IsFailure) return BadRequest(reportOrError.Error);

            //submission.AddReport(reportOrError.Value);

            //var groupMembers = _service.GetGroupMembers(submission.FileSpecification.GenerationUserGroup);
            //if (groupMembers.IsFailure) return BadRequest(groupMembers.Error);

            //var assignee = _uow.WorkItems.GetUserWithLeastAssignments(groupMembers.Value);

            //if (string.IsNullOrWhiteSpace(assignee)) return BadRequest("No user to assign work item");

            //var workItem = WorkItem.Create(WorkItemAction.Generate, assignee);
            //if (workItem.IsFailure) return BadRequest(workItem.Error);

            //reportOrError.Value.ReportState = ReportState.AssignedForGeneration;
            //submission.SubmissionState = SubmissionState.AssignedForGeneration;

            //reportOrError.Value.AddWorkItem(workItem.Value);
            //await _uow.CompleteAsync();
            return Ok();

        }

        [HttpPost, Route("waiver/{submissionid}")]
        public async Task<object> Waiver(int submissionid)
        {
            var submission = await _uow.Submissions.GetByIdAsync(submissionid);
            if (submission == null) return NotFound();

            //var reportOrError = Report.Create(submission);
            //if (reportOrError.IsFailure) return BadRequest(reportOrError.Error);

            //submission.AddReport(reportOrError.Value);

            //reportOrError.Value.Waive();

            //await _uow.CompleteAsync();

            return Ok();
        }

    }
}
