using System.Web.Mvc;

namespace Aden.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            var error = Server.GetLastError();
            ViewBag.Error = Session["Error"];
            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
    }
}