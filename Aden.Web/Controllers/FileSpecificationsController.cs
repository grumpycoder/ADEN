﻿using ADEN.Web.Core;
using ADEN.Web.Data;
using System.Linq;
using System.Web.Http;

namespace Aden.Web.Controllers
{
    [RoutePrefix("api/filespecifications")]
    public class FileSpecificationsController : ApiController
    {
        private readonly UnitOfWork uow;

        public FileSpecificationsController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        [HttpGet]
        public object Get(string search = null, string order = null, int offset = 0, int limit = 10)
        {
            //return Ok(uow.FileSpecifications.GetAllWithReports());
            var specs = uow.FileSpecifications.GetAllWithReportsPaged(search, order, offset, limit);
            var totalRows = uow.FileSpecifications.GetAllWithReportsPaged(search);
            var s = new
            {
                Total = totalRows.Count(),
                Rows = specs
            };
            return Ok(s);
        }


    }
}