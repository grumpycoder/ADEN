using Aden.Core.Data;
using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Web.ViewModels;
using Alsde.Extensions;
using ALSDE.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Mvc;
using Humanizer;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Aden.Web.Controllers.api
{

    [RoutePrefix("api/filespecification")]
    public class FileSpecificationController : ApiController
    {
        private readonly AdenContext _context;
        private readonly IUnitOfWork _uow;

        public FileSpecificationController(AdenContext context, IUnitOfWork uow)
        {
            _context = context;
            _uow = uow;
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            var dto = await _context.FileSpecifications.ProjectTo<FileSpecificationDto>().ToListAsync();
            return Ok(dto);
        }

        [HttpPost, Route("retire/{id}")]
        public async Task<object> Retire(int id)
        {
            var fileSpecification = await _uow.FileSpecifications.GetByIdAsync(id);

            if (fileSpecification == null) return NotFound();

            fileSpecification.Retire();

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

            //Create submission record
            var submission = Submission.Create(fileSpecification);
            fileSpecification.AddSubmission(submission);

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

        [HttpPut, Route("groupmembers")]
        public OkResult AddGroupMember(UpdateGroupMemberDto model)
        {

            var client = new SmtpClient();
            var message = new MailMessage()
            {
                Body = $"Please ADD user {model.Email} to group <br />{model.GroupName.Humanize().ToTitleCase()}",
                To = { "helpdesk@alsde.edu" },
                From = new MailAddress(User.Identity.Name),
                IsBodyHtml = true
            };

            client.Send(message);

            return Ok();
        }

        [HttpDelete, Route("groupmembers")]
        public OkResult DeleteGroupMember(UpdateGroupMemberDto model)
        {

            var client = new SmtpClient();
            var message = new MailMessage()
            {
                Body = $"Please REMOVE user {model.Email} from group <br />{model.GroupName.Humanize().ToTitleCase()}",
                To = { "helpdesk@alsde.edu" },
                From = new MailAddress(User.Identity.Name),
                IsBodyHtml = true
            };

            client.Send(message);

            return Ok();
        }

        [HttpGet, Route("findmembers")]
        public object GetMembers(string username = null)
        {
            var _userService = new IdemUserService();
            var users = _userService.GetUsers(username);
            return Ok(users.ToList());
        }
    }
}
