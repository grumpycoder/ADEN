using System.Configuration;
using System.Web.Mvc;
using Alsde.Security.Identity;

namespace Aden.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly string AccessKey = "34C3A440-B8A8-4235-88AE-EDD2FA2991BC";

        //Callback url from TPA login
        public ActionResult LoginCallback(string token)
        {
            var url = ConfigurationManager.AppSettings["WebServiceUrl"];
            var tokenKey = new TokenKey(token, AccessKey);

            var identity = IdentityManager.TokenSignin(url, tokenKey);

            return RedirectToAction("Submissions", "Home");

        }

    }
}