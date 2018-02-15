using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using ADEN.Web.Core;
using ADEN.Web.Data;
using ADEN.Web.Helpers;
using ADEN.Web.Models;
using ADEN.Web.ViewModels;
using AutoMapper;


namespace ADEN.Web.Controllers
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

            var wi = Mapper.Map<List<WorkItemViewModel>>(workItems);

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
                if (result.Success)
                {
                    wi.Complete();
                    uow.Complete();
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            wi.Complete();
            uow.Complete();
            return Ok();
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

            return Ok(id);
        }

        [HttpPost, Route("generate/{workItemId}")]
        public object Generate(int workItemId)
        {
            try
            {
                var wi = uow.WorkItems.GetById(workItemId);
                var action = wi.Report.FileSpecification.ReportAction;

                var dataTable = new DataTable();
                var ctx = AdenContext.Create();
                using (var connection = new SqlConnection(ctx.Database.Connection.ConnectionString))
                {
                    using (var cmd = new SqlCommand(action, connection))
                    {
                        var adapter =
                            new SqlDataAdapter(cmd) { SelectCommand = { CommandType = CommandType.StoredProcedure } };
                        adapter.Fill(dataTable);
                    }
                }
                var file = dataTable.ToCSVBytes();
                wi.Report.CreateDocument(file, ReportLevel.SCH);

                wi.Complete();

                uow.Complete();

                return Ok(file);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
