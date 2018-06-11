using Aden.Core.Repositories;
using Aden.Web.ViewModels;
using AutoMapper;
using DevExtreme.AspNet.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Aden.Core.Dtos;

namespace Aden.Web.Controllers
{
    [RoutePrefix("api/filespecifications")]
    public class FileSpecificationsController : ApiController
    {
        private readonly IUnitOfWork _uow;

        public FileSpecificationsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var specs = await _uow.FileSpecifications.GetAllAsync();
            var vm = Mapper.Map<List<FileSpecificationDto>>(specs);
            return Ok(vm);
        }

        [HttpPost, Route("retire/{id}")]
        public async Task<object> Retire(int id)
        {
            var spec = await _uow.FileSpecifications.GetByIdAsync(id);

            if (spec == null) return NotFound();

            spec.Retire();

            _uow.Submissions.Delete(spec.Id);

            await _uow.CompleteAsync();

            return Ok(spec);
        }

        [HttpPost, Route("activate/{id}")]
        public async Task<object> Activate(int id)
        {
            var spec = await _uow.FileSpecifications.GetByIdAsync(id);

            if (spec == null) return NotFound();

            spec.Activate();

            await _uow.CompleteAsync();

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
            var spec = _uow.FileSpecifications.GetById(model.Id);

            Mapper.Map(model, spec);

            _uow.Complete();

            return Ok(model);
        }

    }
}
