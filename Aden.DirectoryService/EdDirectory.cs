using Aden.Core;
using Aden.DirectoryService.Models.WebService;
using Aden.Extensions;

namespace Aden.DirectoryService
{
    public class EdDirectory
    {
        public string Url { get; private set; }

        private EdDirectory() { }

        public EdDirectory(string url)
        {
            Url = url;
        }

        public Core.Person GetPersonDetail(string token, string accessKey)
        {

            var idemService = new IdemService.Service { Url = Url };

            var data = idemService.GetUserDetail(accessKey, token);

            var personXml = data.ChildNodes[0].FirstChild;
            var personContract = personXml.OuterXml.ParseToClass<PersonContract>();


            //TODO: Convert to CoreLib Person and return

            var person = new Person()
            {
                FirstName = personContract.FirstName,
                MiddleName = personContract.MiddleName,
                LastName = personContract.LastName,
                Dob = personContract.Dob,
                IdentityGuid = personContract.IdentityGuid,
                IdentityId = personContract.IdentityId,
                Email = personContract.Email,
                Id = personContract.Id,
            };

            personContract.AimGroups.ForEach(x => person.Groups.Add(new Group() { Description = x.Description, GroupViewKey = x.GroupViewKey }));

            return person;
        }
    }
}
