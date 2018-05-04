using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Aden.Core.Data;
using Aden.Core.Repositories;
using Aden.Web.ViewModels;
using AutoMapper;
using DevExtreme.AspNet.Mvc;

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
        public object Get(DataSourceLoadOptions loadOptions)
        {
            //return Ok(uow.FileSpecifications.GetAllWithReports());
            //var specs = uow.FileSpecifications.GetWithReportsPaged(search, order, offset, limit);
            var specs = uow.FileSpecifications.GetAllWithReports();

            var vm = Mapper.Map<List<FileSpecificationViewModel>>(specs);
            return Ok(vm);

            //var s = new
            //{
            //    Total = totalRows.Count(),
            //    Rows = si
            //};
            //return Ok(s);
        }

        [HttpGet, Route("all")]
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

        [HttpPost, Route("Update")]
        public object Update(FileSpecificationEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var spec = uow.FileSpecifications.GetById(model.Id);

            Mapper.Map(model, spec);

            uow.Complete();

            return Ok(model);
        }

        [HttpPost, Route("retire/{id}")]
        public object Retire(int id)
        {
            var spec = uow.FileSpecifications.GetById(id);

            if (spec == null) return NotFound();

            spec.Retire();

            uow.Submissions.Delete(spec.Id);

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
