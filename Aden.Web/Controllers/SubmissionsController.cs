using System.Web.Http;
using ADEN.Web.Core;
using ADEN.Web.Data;

namespace Aden.Web.Controllers
{
    public class SubmissionsController : ApiController
    {
        private readonly UnitOfWork uow;

        public SubmissionsController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }
    }
}
