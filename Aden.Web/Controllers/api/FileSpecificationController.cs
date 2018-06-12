using Aden.Core.Dtos;
using Aden.Core.Repositories;
using AutoMapper;
using DevExtreme.AspNet.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/filespecification")]
    public class FileSpecificationController : ApiController
    {
        private readonly IUnitOfWork _uow;

        public FileSpecificationController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [System.Web.Http.HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var specs = await _uow.FileSpecifications.GetAllAsync();
            var vm = Mapper.Map<List<FileSpecificationDto>>(specs);
            return Ok(vm);
        }

        [HttpPost, Route("retire/{id}")]
        public async Task<object> Retire(int id)
        {
            var fileSpecification = await _uow.FileSpecifications.GetByIdAsync(id);

            if (fileSpecification == null) return NotFound();

            fileSpecification.Retire();

            //TODO: only delete current data year on file specification child records
            _uow.Submissions.Delete(fileSpecification.Id);

            await _uow.CompleteAsync();

            return Ok(fileSpecification);
        }

        [HttpPost, Route("activate/{id}")]
        public async Task<object> Activate(int id)
        {
            var fileSpecification = await _uow.FileSpecifications.GetByIdAsync(id);

            if (fileSpecification == null) return NotFound();

            //TODO: If activate then need to create submission record 
            fileSpecification.Activate();

            await _uow.CompleteAsync();

            return Ok(fileSpecification);
        }

        [HttpPut, Route("{id}")]
        public object Update(int id, UpdateFileSpecificationDto model)
        {
            var fileSpecification = _uow.FileSpecifications.GetById(id);

            Mapper.Map(model, fileSpecification);

            _uow.Complete();

            return Ok(model);
        }
    }
}
