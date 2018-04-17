using System.Configuration;
using System.Text;
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


        public ActionResult Signout()
        {
            IdentityManager.IdentitySignout();
            var sb = new StringBuilder();
            var env = ConfigurationManager.AppSettings["ASPNET_ENV"].ToLower();
            var viewKey = ConfigurationManager.AppSettings["ALSDE_AIM_ApplicationViewKey"];

            //TODO: Refacto magic string out
            sb.AppendFormat("http://devaim.alsde.edu/aim/applicationinventory.aspx?logout={0}", viewKey);

            if (HttpContext.Request.IsSecureConnection) sb.Replace("http", "https");

            //TODO: Refactor to utility class maybe
            switch (env)
            {
                case "test":
                    {
                        sb.Replace("dev", "test");
                        break;
                    }
                case "stage":
                    {
                        sb.Replace("dev", "stage");
                        break;
                    }
                case "production":
                    {
                        sb.Replace("dev", "");
                        break;
                    }
            }
            return Redirect(sb.ToString());
        }

    }
}