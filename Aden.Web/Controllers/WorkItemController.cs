using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Aden.Core.Data;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Web.ViewModels;
using AutoMapper;


namespace Aden.Web.Controllers
{
    [RoutePrefix("api/wi")]
    public class WorkItemController : ApiController
    {
        private readonly UnitOfWork uow;

        public WorkItemController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        [HttpGet, Route("{username}")]
        public async Task<object> Get(string username)
        {
            username = User.Identity.Name ?? username;

            var workitems = await uow.WorkItems.GetActiveAsync(username);
            var completedWorkItems = await uow.WorkItems.GetCompletedAsync(username);
            var retrievableWorkItems = await uow.WorkItems.GetCompletedAsync(username);

            var wi = Mapper.Map<List<WorkItemViewModel>>(workitems);
            var wi2 = Mapper.Map<List<WorkItemViewModel>>(completedWorkItems);
            var wi3 = Mapper.Map<List<WorkItemViewModel>>(retrievableWorkItems);

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
            var workitems = await uow.WorkItems.GetActiveAsync(username);

            var wi = Mapper.Map<List<WorkItemViewModel>>(workitems);

            return Ok(wi);
        }

        [HttpGet, Route("completed/{username}")]
        public async Task<object> Completed(string username)
        {
            var workItems = await uow.WorkItems.GetCompletedAsync(username);

            var wi = Mapper.Map<List<WorkItemViewModel>>(workItems.OrderByDescending(w => w.CanCancel).ThenByDescending(w => w.AssignedDate));

            return Ok(wi);
        }

        [HttpPost, Route("complete/{id}")]
        public async Task<object> Complete(int id)
        {
            var wi = await uow.WorkItems.GetByIdAsync(id);
            if (wi == null) return NotFound();

            //TODO: Remove logic from controller
            if (wi.WorkItemAction == WorkItemAction.Generate)
            {
                var result = uow.GenerateDocuments(wi.ReportId ?? 0);
                if (!result.Success) return BadRequest(result.Message);

                wi.Complete();
                uow.Complete();

                var vm = Mapper.Map<WorkItemViewModel>(wi);
                return Ok(vm);
            }

            try
            {
                wi.Complete();
                await uow.CompleteAsync();
                var vm = Mapper.Map<WorkItemViewModel>(wi);
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
            var wi = uow.WorkItems.GetById(id);
            if (wi == null) return NotFound();

            wi.SetAction(WorkItemAction.SubmitWithError);

            wi.Complete();
            uow.Complete();
            var vm = Mapper.Map<WorkItemViewModel>(wi);

            return Ok(vm);
        }

        [HttpPost, Route("undo/{id}")]
        public async Task<object> Undo(int id)
        {
            var wi = await uow.WorkItems.GetByIdAsync(id);
            if (wi == null) return NotFound();

            wi.Report.CancelWorkItems();
            wi.Report.StartNewWork();

            uow.Documents.DeleteReportDocuments(wi.ReportId ?? 0);

            uow.Complete();

            var vm = Mapper.Map<WorkItemViewModel>(wi);

            return Ok(vm);
        }


        [HttpPost, Route("reassign")]
        public async Task<object> Reassign([FromBody]ReassignmentViewModel model)
        {
            var workItem = await uow.WorkItems.GetByIdAsync(model.WorkItemId);
            if (workItem == null) return NotFound();

            var wi = workItem.Report.ReassignWorkItem(workItem, model.AssignedUser);

            await uow.CompleteAsync();

            return Ok(wi);
        }

    }
}
