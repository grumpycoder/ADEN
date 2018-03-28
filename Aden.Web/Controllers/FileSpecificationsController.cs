using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Aden.Core.Data;
using Aden.Core.Repositories;
using Aden.Web.ViewModels;
using AutoMapper;

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

            var si = Mapper.Map<List<FileSpecificationViewModel>>(specs);
            var s = new
            {
                Total = totalRows.Count(),
                Rows = si
            };
            return Ok(s);
        }

        public object Put(FileSpecificationEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var spec = uow.FileSpecifications.GetById(model.Id);

            Mapper.Map(model, spec);

            uow.Complete();

            return Ok();
        }

        [HttpPost, Route("retire/{id}")]
        public object Retire(int id)
        {
            var spec = uow.FileSpecifications.GetById(id);

            if (spec == null) return NotFound();

            spec.Retire();

            uow.Complete();

            return Ok(spec);
        }

        [HttpPost, Route("activate/{id}")]
        public object Activate(int id)
        {
            var spec = uow.FileSpecifications.GetById(id);

            if (spec == null) return NotFound();

            spec.Activate();

            uow.Complete();

            return Ok(spec);
        }
    }
}
