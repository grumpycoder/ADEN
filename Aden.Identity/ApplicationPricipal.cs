using System.Security.Claims;

namespace Aden.Identity
{
    public class ApplicationPricipal : ClaimsPrincipal
    {
        public ApplicationPricipal(ApplicationUser identity)
            : base(identity)
        {
        }

        public ApplicationPricipal(ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        {
        }
    }
}