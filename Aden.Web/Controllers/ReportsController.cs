using System;
using System.Collections.Generic;
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

        [HttpGet, Route("{datayear:int}")]
        public async Task<object> Get(int datayear)
        {
            var reports = await uow.Reports.GetByFileSpecificationAsync(datayear);
            var reportList = Mapper.Map<List<ReportViewModel>>(reports);
            return Ok(reportList);
        }

        [HttpGet, Route("{datayear:int}/{filenumber}")]
        public async Task<object> Get(int datayear, string filenumber)
        {
            var reports = await uow.Reports.GetByFileSpecificationAsync(datayear, filenumber);
            var reportList = Mapper.Map<List<ReportViewModel>>(reports);
            return Ok(reportList);
        }

        [HttpPost, Route("create/{submissionid}")]
        public async Task<object> Create(int submissionid)
        {
            var submission = uow.Submissions.GetById(submissionid);

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
