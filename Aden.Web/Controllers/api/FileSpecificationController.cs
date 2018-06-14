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

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var specs = await _uow.FileSpecifications.GetAllAsync();
            var dto = Mapper.Map<List<FileSpecificationDto>>(specs);
            return Ok(dto);
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

            var dto = Mapper.Map<FileSpecificationDto>(fileSpecification);

            return Ok(dto);
        }

        [HttpPost, Route("activate/{id}")]
        public async Task<object> Activate(int id)
        {
            var fileSpecification = await _uow.FileSpecifications.GetByIdAsync(id);

            if (fileSpecification == null) return NotFound();

            //TODO: If activate then need to create submission record 
            fileSpecification.Activate();

            await _uow.CompleteAsync();

            var dto = Mapper.Map<FileSpecificationDto>(fileSpecification);
            return Ok(dto);
        }

        [HttpPut, Route("{id}")]
        public object Update(int id, UpdateFileSpecificationDto dto)
        {
            var fileSpecification = _uow.FileSpecifications.GetById(id);

            Mapper.Map(dto, fileSpecification);

            _uow.Complete();

            return Ok(dto);
        }
    }
}
