using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Aden.Core.Data;
using Aden.Core.Repositories;
using Aden.Web.ViewModels;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;

namespace Aden.Web.Controllers
{
    [RoutePrefix("api/submissions")]
    public class SubmissionsController : ApiController
    {
        private readonly UnitOfWork uow;

        public SubmissionsController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        [HttpGet, Route("all")]
        public object GetPaged(DataSourceLoadOptions loadOptions)
        {
            //var submissions = uow.Submissions.GetAllWithReports();
            //var order = loadOptions.Sort.ToString();
            //var offset = loadOptions.Skip;
            //var limit = loadOptions.Take;
            var submissions = uow.Submissions.GetAllWithReportsPaged();


            var rows = Mapper.Map<List<SubmissionViewModel>>(submissions);
            return Ok(DataSourceLoader.Load(rows, loadOptions));
        }

        [HttpGet]
        public object Get(string search = null, string order = null, int offset = 0, int limit = 10)
        {
            var submissions = uow.Submissions.GetAllWithReportsPaged(search, order, offset, limit);
            var totalRows = uow.Submissions.GetAllWithReportsPaged(search);

            var rows = Mapper.Map<List<SubmissionViewModel>>(submissions);
            var vm = new
            {
                Total = totalRows.Count(),
                Rows = rows
            };
            return Ok(vm);
        }


    }
}
