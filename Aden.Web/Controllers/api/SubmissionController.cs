﻿using Aden.Core.Dtos;
using Aden.Core.Repositories;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/submission")]
    [Authorize]
    public class SubmissionController : ApiController
    {
        private readonly IUnitOfWork _uow;

        private readonly string _globalAdministrators =
            ConfigurationManager.AppSettings["GlobalAdministratorsGroupName"];

        public SubmissionController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            //TODO: Refactor isGlobalAdmin variable
            //var isGlobalAdmin = User.IsInRole(_globalAdministrators);
            var isGlobalAdmin = User.IsInRole(Constants.GlobalAdministratorGroup);

            var section = ((ClaimsPrincipal)User).Claims.FirstOrDefault(c => c.Type == "Section")?.Value;

            var submissions = await _uow.Submissions.GetBySectionWithReportsAsync(!isGlobalAdmin ? section : string.Empty);

            var rows = Mapper.Map<List<SubmissionDto>>(submissions);

            return Ok(DataSourceLoader.Load(rows, loadOptions));

        }


    }
}
