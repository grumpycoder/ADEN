using ALSDE.Dtos;
using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace Aden.Core.Services
{
    public class InMemoryMembershipService : IMembershipService
    {
        public Result<List<string>> GetGroupMembers(string groupName)
        {
            var list = new List<string>()
            {
                "mlawrence@alsde.edu",
                "jhall@alsde.edu",
                "mkong@alsde.edu",
                "jking@alsde.edu"
            };

            return Result.Ok(list);
        }

        public Result<List<string>> GetGroups(string groupName)
        {
            throw new System.NotImplementedException();
        }

        public bool GroupExists(string groupName)
        {
            throw new System.NotImplementedException();
        }

        Result<List<GroupDto>> IMembershipService.GetGroups(string groupName)
        {
            throw new System.NotImplementedException();
        }
    }
}
