using Alsde.Extensions;
using Alsde.Security.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Aden.Web.Helpers;
using Alsde.Entity;
using Alsde.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Aden.Web.Controllers
{

    public class AccountController : Controller
    {
        public IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        //private readonly string _accessKey = AppSettings.Get<string>(Constants.TpaAccessKey);

        //Callback url from TPA login
        public ActionResult LoginCallback(string token)
        {
            var tokenKey = new TokenKey(token, Constants.TpaAccessKey);

            var dir = new EdDirectory(AppSettings.Get<string>("WebServiceUrl"));
            var person = dir.GetPersonDetail(tokenKey.Token, tokenKey.AccessKey);

            var identity = CreateClaimsIdentity(person);

            AuthenticationManager.SignIn(identity);
            //var identity = IdentityManager.TokenSignin(Constants.WebServiceUrl, tokenKey);

            //if (identity == null) throw new Exception("No identity returned from Token signin");

            //// Add custom claims to User to store Section information
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

        private static ClaimsIdentity CreateClaimsIdentity(Person person)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, person.Email),
                new Claim(ClaimTypes.Name, person.Email),
                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", person.Email),
            };

            var list = person.Groups.Select(g => g.GroupViewKey);

            foreach (var group in person.Groups)
            {
                claims.Add(new Claim(ClaimTypes.Role, group.GroupViewKey));
            }

            return new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}