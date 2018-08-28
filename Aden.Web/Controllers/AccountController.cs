using Alsde.Extensions;
using Alsde.Security.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Aden.Web.Controllers
{
    public class AccountController : Controller
    {
        //private readonly string _accessKey = AppSettings.Get<string>(Constants.TpaAccessKey);

        //Callback url from TPA login
        public ActionResult LoginCallback(string token)
        {
            var tokenKey = new TokenKey(token, Constants.TpaAccessKey);

            var identity = IdentityManager.TokenSignin(Constants.WebServiceUrl, tokenKey);

            if (identity == null) throw new Exception("No identity returned from Token signin");

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
            //var env = Constants.Environment.ToLower();
            var env = Constants.Environment.ToLower() == "production" ? string.Empty : Constants.Environment.ToLower();

            //if (env == "production") env = string.Empty;

            var logoutUrl = $"https://{env}{Constants.LogoutUrl}{Constants.AimApplicationViewKey}";

            return Redirect(logoutUrl);
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

    }
}