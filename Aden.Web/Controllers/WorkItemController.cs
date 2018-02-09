using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using ADEN.Web.Core;
using ADEN.Web.Data;
using ADEN.Web.Helpers;
using ADEN.Web.Models;


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
