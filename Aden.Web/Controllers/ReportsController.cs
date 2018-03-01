using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Aden.Web.ViewModels;
using ADEN.Web.Core;
using ADEN.Web.Data;
using ADEN.Web.Models;
using AutoMapper;

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

        [HttpGet, Route("{filespecificationId}")]
        public object Get(string filespecificationId = null)
        {
            return Ok(uow.Reports.GetByFileSpecificationNumber(filespecificationId));
        }

        [HttpGet, Route("{filespecificationId}/{datayear}")]
        public object Get(string filespecificationId, int datayear)
        {
            var reports = uow.Reports.GetByFileSpecificationNumber(filespecificationId, datayear); 

            var reportList = Mapper.Map<List<ReportViewModel>>(reports);
            return Ok(reportList);
        }

        [HttpGet, Route("search")]
        public object Search(string search = null, string order = "asc", int offset = 0, int limit = 10)
        {
            var reports = uow.Reports.GetByFileSpecificationNumberPaged(search, order, offset, limit);
            var totalRows = uow.FileSpecifications.GetAllWithReportsPaged(search);

            var reportList = Mapper.Map<List<ReportViewModel>>(reports);

            var s = new
            {
                Total = totalRows.Count(),
                Rows = reportList
            };
            return Ok(s);
        }

        [HttpPost, Route("create/{id}")]
        public object Create(int id)
        {
            var spec = uow.FileSpecifications.GetById(id);

            if (spec == null) return NotFound();

            //var report = Report.Create(spec);
            var report = new Report();
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

            //var report = Report.Create(spec);
            var report = new Report();
            spec.AddReport(report);

            report.Waive();

            uow.Complete();

            return Ok();
        }

    }
}
