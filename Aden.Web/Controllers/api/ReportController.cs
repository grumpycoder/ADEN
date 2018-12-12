using Aden.Core.Data;
using Aden.Core.Dtos;
using Aden.Core.Repositories;
using Aden.Core.Services;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aden.Web.Controllers.api
{
    [RoutePrefix("api/report")]
    public partial class ReportController : ApiController
    {
        private readonly AdenContext _context;
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;
        private readonly IMembershipService _membershipService;

        public ReportController(AdenContext context, IUnitOfWork uow, INotificationService notificationService, IMembershipService membershipService)
        {
            _context = context;
            _uow = uow;
            _notificationService = notificationService;
            _membershipService = membershipService;
        }

        [HttpGet, Route("{datayear:int}/{filenumber}")]
        public async Task<object> Get(int datayear, string filenumber)
        {
            var dto = _context.Reports
                .Where(f => (f.Submission.FileSpecification.FileNumber == filenumber &&
                             f.Submission.DataYear == datayear) || string.IsNullOrEmpty(filenumber))
                .OrderByDescending(x => x.Id).ProjectTo<ReportViewDto>();

            return Ok(dto);
        }

        [HttpGet, Route("document/{id:int}")]
        public async Task<object> Document(int id)
        {

            var dto = await _context.ReportDocuments.Where(d => d.Id == id).Select(r => new ReportDocumentDto()
            {
                Filename = r.Filename,
                Version = r.Version,
                Id = r.Id,
                FileData = r.FileData,
                FileSize = r.FileSize

            }).FirstOrDefaultAsync();


            return Ok(dto);
        }


    }
}
