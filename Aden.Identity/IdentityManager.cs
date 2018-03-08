using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using Aden.Core;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Aden.Identity
{
    public static class IdentityManager
    {
        private static IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        public static void IdentitySignin(Person person, string providerKey = null, bool isPersistent = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, person.Id),
                new Claim(ClaimTypes.Name, person.Email),
                new Claim("person", person.LastName)
            };

            foreach (var group in person.Groups)
            {
                claims.Add(new Claim(ClaimTypes.Role, group.GroupViewKey));
            }

            var identity = new ApplicationUser(claims, DefaultAuthenticationTypes.ApplicationCookie);

            //foreach (var group in person.AimGroups)
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Role, group.GroupViewKey));
            //}

            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            }, identity);

            FormsAuthentication.SetAuthCookie(identity.Name, true);

            HttpContext.Current.User = new ApplicationPricipal(AuthenticationManager.AuthenticationResponseGrant.Principal);
        }

        public static void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}