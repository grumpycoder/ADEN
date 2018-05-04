using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var specs = await uow.FileSpecifications.GetAllAsync();
            var vm = Mapper.Map<List<FileSpecificationViewModel>>(specs);
            return Ok(vm);
        }

        [HttpPost, Route("retire/{id}")]
        public async Task<object> Retire(int id)
        {
            var spec = await uow.FileSpecifications.GetByIdAsync(id);

            if (spec == null) return NotFound();

            spec.Retire();

            uow.Submissions.Delete(spec.Id);

            await uow.CompleteAsync();

            return Ok(spec);
        }

        [HttpPost, Route("activate/{id}")]
        public async Task<object> Activate(int id)
        {
            var spec = await uow.FileSpecifications.GetByIdAsync(id);

            if (spec == null) return NotFound();

            spec.Activate();

            await uow.CompleteAsync();

            return Ok(spec);
        }

        //TODO: Needs implementation on edit view modal
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

    }
}
