using ADEN.Web.Core;
using ADEN.Web.Data;
using ADEN.Web.Models;
using System.Web.Http;

namespace Aden.Web.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private readonly UnitOfWork uow;

        public ReportsController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        [HttpGet]
        public object Get(string id)
        {
            return Ok(uow.Reports.GetByFileSpecificationNumber(id));
        }

        [HttpPost, Route("create/{id}")]
        public object Create(int id)
        {
            var spec = uow.FileSpecifications.GetById(id);

            if (spec == null) return NotFound();

            var report = Report.Create(spec);
            spec.AddReport(report);

            report.StartNewWork();

            uow.Complete();

            return Ok();
        }

        [HttpPost, Route("waiver/{id}")]
        public object Waiver(int id)
        {
            var spec = uow.FileSpecifications.GetById(id);

            if (spec == null) return NotFound();

            var report = Report.Create(spec);
            spec.AddReport(report);

            report.Waive();

            uow.Complete();

            return Ok();
        }

    }
}
