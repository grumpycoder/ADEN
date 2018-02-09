using ADEN.Web.Core;
using ADEN.Web.Data;
using System.Web.Mvc;

namespace Aden.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork uow;

        public HomeController()
        {
            var context = AdenContext.Create();
            uow = new UnitOfWork(context);
        }

        public ActionResult FileSpecifications()
        {
            var specs = uow.FileSpecifications.GetAllWithReports();
            return View(specs);
        }

    }
}