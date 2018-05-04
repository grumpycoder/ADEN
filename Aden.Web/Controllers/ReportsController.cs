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
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private readonly UnitOfWork uow;

        public ReportsController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        //[HttpGet, Route("{filespecificationId}")]
        //public object Get(string filespecificationId = null)
        //{
        //    return Ok(uow.Reports.GetByFileSpecificationNumber(filespecificationId));
        //}

        [HttpGet, Route("{datayear:int}")]
        public object Get(int datayear)
        {
            var reports = uow.Reports.GetFileSpecificationsByDataYear(datayear);
            var reportList = Mapper.Map<List<ReportViewModel>>(reports);
            return Ok(reportList);
        }

        [HttpGet, Route("{datayear:int}/{filenumber}")]
        public object Get(int datayear, string filenumber)
        {
            //var reports = uow.Reports.GetByFileSpecificationNumber(filespecificationId, datayear);
            var reports = uow.Reports.GetFileSpecifications(datayear, filenumber);

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

        [HttpPost, Route("create/{submissionid}")]
        public async Task<object> Create(int submissionid)
        {
            var submission = await uow.Submissions.GetByIdAsync(submissionid);

            if (submission == null) return NotFound();

            try
            {
                var report = Report.Create(submission);
                submission.AddReport(report);
                report.StartNewWork();
                await uow.CompleteAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost, Route("waiver/{submissionid}")]
        public async Task<object> Waiver(int submissionid)
        {
            var submission = await uow.Submissions.GetByIdAsync(submissionid);

            if (submission == null) return NotFound();

            var report = new Report();
            submission.AddReport(report);

            report.Waive();

            await uow.CompleteAsync();

            return Ok();
        }

    }
}
