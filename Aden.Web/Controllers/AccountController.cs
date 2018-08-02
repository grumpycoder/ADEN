using Aden.Web.Helpers;
using Alsde.Extensions;
using Alsde.Security.Identity;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;

namespace Aden.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _accessKey = AppSettings.Get<string>(Constants.TpaAccessKey);

        //Callback url from TPA login
        public ActionResult LoginCallback(string token)
        {
            var url = AppSettings.Get<string>(Constants.WebServiceUrlKey);
            var tokenKey = new TokenKey(token, _accessKey);

            var identity = IdentityManager.TokenSignin(url, tokenKey);

            if (identity == null) throw new Exception("No idenity returned from Token signin");
            // Add custom claims to User to store Section information
            var claims = identity.Claims.ToList();
            var claim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value.ToLower().Contains("section"));
            var isAdministrator = claims.Any(c => c.Value.ToLower().Contains("administrator"));

            if (claim != null)
            {
                var section = claim.Value.SplitCamelCase().Split(' ').ToList();
                var idxSection = section.IndexOf("Section");
                var idxApp = section.IndexOf("App");

                var sectionName = string.Join(" ", section.Skip(idxApp + 1).Take(idxSection - idxApp - 1).ToList());
                identity.AddClaim(new Claim("Section", sectionName));
            }

            if (isAdministrator) return RedirectToAction("Submissions", "Home");

            return RedirectToAction("Assignments", "Home");

        }


        public ActionResult Signout()
        {
            IdentityManager.IdentitySignout();
            var sb = new StringBuilder();
            var env = ConfigurationManager.AppSettings["ASPNET_ENV"].ToLower();
            var viewKey = ConfigurationManager.AppSettings["ALSDE_AIM_ApplicationViewKey"];

            //TODO: Refactor magic string out of signout
            sb.AppendFormat("http://devaim.alsde.edu/aim/applicationinventory.aspx?logout={0}", viewKey);

            if (HttpContext.Request.IsSecureConnection) sb.Replace("http", "https");

            //TODO: Refactor to utility or factory class 
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

        public ActionResult Unauthorized()
        {
            return View();
        }

    }
}