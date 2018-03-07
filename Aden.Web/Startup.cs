using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Aden.Web.Startup))]
namespace Aden.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }


        private void ConfigureAuth(IAppBuilder app)
        {
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),

                // Only do this for demo!!
                AllowInsecureHttp = true
            };
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(
                new OAuthBearerAuthenticationOptions());
        }

    }

    public class ApplicationOAuthServerProvider
        : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(
            OAuthValidateClientAuthenticationContext context)
        {
            // This call is required...
            // but we're not using client authentication, so validate and move on...
            await Task.FromResult(context.Validated());
        }


        public override async Task GrantResourceOwnerCredentials(
            OAuthGrantResourceOwnerCredentialsContext context)
        {
            // DEMO ONLY: Pretend we are doing some sort of REAL checking here:
            if (context.Password != "password")
            {
                context.SetError(
                    "invalid_grant", "The user name or password is incorrect.");
                context.Rejected();
                return;
            }

            // Create or retrieve a ClaimsIdentity to represent the 
            // Authenticated user:
            ClaimsIdentity identity =
                new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("user_name", context.UserName));

            // Identity info will ultimately be encoded into an Access Token
            // as a result of this call:
            context.Validated(identity);
        }
    }
}