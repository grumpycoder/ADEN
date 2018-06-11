using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Web.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aden.Web.Controllers
{
    [RoutePrefix("api/wi")]
    public class WorkItemController : ApiController
    {
        private readonly IUnitOfWork _uow;

        public WorkItemController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet, Route("{username}")]
        public async Task<object> Get(string username)
        {
            username = User.Identity.Name ?? username;

            var workitems = await _uow.WorkItems.GetActiveAsync(username);
            var completedWorkItems = await _uow.WorkItems.GetCompletedAsync(username);
            var retrievableWorkItems = await _uow.WorkItems.GetCompletedAsync(username);

            var wi = Mapper.Map<List<WorkItemDto>>(workitems);
            var wi2 = Mapper.Map<List<WorkItemDto>>(completedWorkItems);
            var wi3 = Mapper.Map<List<WorkItemDto>>(retrievableWorkItems);

            var s = new
            {
                WorkItems = wi,
                CompletedWorkItems = wi2,
                RetrievableWorkItems = wi3
            };
            return Ok(s);

        }

        [HttpGet, Route("current/{username}")]
        public async Task<object> Current(string username)
        {
            var workitems = await _uow.WorkItems.GetActiveAsync(username);

            var wi = Mapper.Map<List<WorkItemDto>>(workitems);

            return Ok(wi);
        }

        [HttpGet, Route("completed/{username}")]
        public async Task<object> Completed(string username)
        {
            var workItems = await _uow.WorkItems.GetCompletedAsync(username);

            var wi = Mapper.Map<List<WorkItemDto>>(workItems.OrderByDescending(w => w.CanCancel).ThenByDescending(w => w.AssignedDate));

            return Ok(wi);
        }

        [HttpPost, Route("complete/{id}")]
        public async Task<object> Complete(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);
            if (wi == null) return NotFound();


            //TODO: Remove logic from controller
            if (wi.WorkItemAction == WorkItemAction.Generate)
            {
                var result = _uow.GenerateDocuments(wi.ReportId ?? 0);
                if (result.IsFailure) return BadRequest(result.Error);

                wi.Finish();
                _uow.Complete();

                var vm = Mapper.Map<WorkItemDto>(wi);
                return Ok(vm);
            }

            try
            {
                wi.Finish();
                await _uow.CompleteAsync();
                var vm = Mapper.Map<WorkItemDto>(wi);
                return Ok(vm);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost, Route("completewitherror/{id}")]
        public object CompleteWithError(int id)
        {
            var wi = _uow.WorkItems.GetById(id);
            if (wi == null) return NotFound();

            wi.SetAction(WorkItemAction.SubmitWithError);

            wi.Finish();
            _uow.Complete();
            var vm = Mapper.Map<WorkItemViewModel>(wi);

            return Ok(vm);
        }

        [HttpPost, Route("undo/{id}")]
        public async Task<object> Undo(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);
            if (wi == null) return NotFound();

            wi.Report.CancelWorkItems();
            wi.Report.StartNewWork();

            _uow.Documents.DeleteReportDocuments(wi.ReportId ?? 0);

            _uow.Complete();

            var vm = Mapper.Map<WorkItemViewModel>(wi);

            return Ok(vm);
        }


        [HttpPost, Route("reassign")]
        public async Task<object> Reassign([FromBody]ReassignmentViewModel model)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(model.WorkItemId);
            if (workItem == null) return NotFound();

            var wi = workItem.Report.ReassignWorkItem(workItem, model.AssignedUser);

            await _uow.CompleteAsync();

            return Ok(wi);
        }

    }
}
