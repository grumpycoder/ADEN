﻿using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Web;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(Aden.Web.Startup))]
namespace Aden.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                //CookieName = "Aden",
                CookieDomain = "alsde.edu",
                //CookieHttpOnly = true,
                //CookieSecure = CookieSecureOption.Never,
                //LoginPath = new PathString("/Account/LoginCallback"),
                //Provider = new CookieAuthenticationProvider()
                //{
                //    OnApplyRedirect = ctx =>
                //    {
                //        if (!IsApiRequest(ctx.Request))
                //        {
                //            ctx.Response.Redirect(ctx.RedirectUri);
                //        }
                //    }
                //}
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

        }

        private static bool IsApiRequest(IOwinRequest request)
        {
            var apiPath = VirtualPathUtility.ToAbsolute("~/api/");
            return request.Uri.LocalPath.StartsWith(apiPath);
        }

    }

}