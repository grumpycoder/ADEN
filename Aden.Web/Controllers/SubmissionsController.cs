using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
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

        private readonly string globalAdministrators =
            ConfigurationManager.AppSettings["GlobalAdministratorsGroupName"];

        public SubmissionsController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        [HttpGet, Route("all")]
        public object GetPaged(DataSourceLoadOptions loadOptions)
        {
            var claim = (User as ClaimsPrincipal).Claims.Where(c => c.Type == "Section").Select(c => c.Value)
                .SingleOrDefault();

            var isGlobalAdmin = User.IsInRole(globalAdministrators);

            var submissions = uow.Submissions.GetAllBySectionWithReportsPaged(!isGlobalAdmin ? claim : string.Empty);

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
