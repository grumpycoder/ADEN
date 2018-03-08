using System.Collections.Generic;
using System.Security.Claims;
using Aden.Core;

namespace Aden.Identity
{
    public class ApplicationUser : ClaimsIdentity
    {

        //public ApplicationUserIds ApplicationUserIds { get; set; }
       
        //public List<AimGroup> AimGroups { get; set; }
        public string Email { get; set; }
        //public List<PhoneNumber> PhoneNumbers { get; set; }
        public List<Site> Sites { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Dob { get; set; }
        public string IdentityId { get; set; }
        public string IdentityGuid { get; set; }

        public ApplicationUser()
        {

        }

        public ApplicationUser(IEnumerable<Claim> claims, string authenticationType)
            : base(claims, authenticationType: authenticationType)
        {
        }
    }
}