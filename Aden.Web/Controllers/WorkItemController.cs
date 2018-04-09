﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public object Get(string username)
        {
            username = User.Identity.Name ?? username;

            var workitems = uow.WorkItems.GetActiveByUser(username);
            var completedWorkItems = uow.WorkItems.GetCompletedByUser(username);
            var retrievableWorkItems = uow.WorkItems.GetCompletedByUser(username);

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

        [HttpGet, Route("currentassignments/{username}")]
        public object CurrentAssignments(string username)
        {
            var workitems = uow.WorkItems.GetActiveByUser(username);

            var wi = Mapper.Map<List<WorkItemViewModel>>(workitems);

            return Ok(wi);
        }

        [HttpGet, Route("completedassignments/{username}")]
        public object CompletedAssignments(string username)
        {
            var workItems = uow.WorkItems.GetCompletedByUser(username);

            var wi = Mapper.Map<List<WorkItemViewModel>>(workItems.OrderByDescending(w => w.CanCancel).ThenByDescending(w => w.AssignedDate));

            return Ok(wi);
        }

        [HttpPost, Route("complete/{id}")]
        public object Complete(int id)
        {
            var wi = uow.WorkItems.GetById(id);
            if (wi == null) return NotFound();

            if (wi.WorkItemAction == WorkItemAction.Generate)
            {
                var result = uow.GenerateDocuments(wi.ReportId ?? 0);
                if (!result.Success) return BadRequest(result.Message);

                wi.Complete();
                uow.Complete();
                return Ok(result.Message);
            }

            try
            {
                wi.Complete();
                uow.Complete();
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
            return Ok("complete with error");
        }

        [HttpPost, Route("undo/{id}")]
        public object Undo(int id)
        {
            var wi = uow.WorkItems.GetByIdWithDetails(id);
            if (wi == null) return NotFound();

            wi.Report.CancelWorkItems();
            wi.Report.StartNewWork();

            uow.Documents.DeleteReportDocuments(wi.ReportId ?? 0);

            uow.Complete();

            return Ok(wi);
        }

    }
}
