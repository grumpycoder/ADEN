using System.Configuration;
using System.Web.Mvc;
using Alsde.Security.Identity;
using Alsde.Services;

namespace Aden.Web.Controllers
{
    public class AccountController : Controller
    {

        private readonly string AccessKey = "34C3A440-B8A8-4235-88AE-EDD2FA2991BC";

        public ActionResult LoginCallback(string token)
        {
            var url = ConfigurationManager.AppSettings["WebServiceUrl"];
            var person = new EdDirectory(url).GetPersonDetail(token, AccessKey);

            if (person == null) return RedirectToAction("Error", "Home");

            IdentityManager.IdentitySignin(person);

            return RedirectToAction("Submissions", "Home");

        }

    }
}