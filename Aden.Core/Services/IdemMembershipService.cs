//using ALSDE.Idem;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Aden.Core.Services
{
    public class IdemMembershipService : IMembershipService
    {

        public IdemMembershipService()
        {

        }

        public Result<List<string>> GetGroupMembers(string groupName)
        {
            //if (string.IsNullOrWhiteSpace(groupName)) return Result.Fail<List<string>>("Group name should not be empty");

            //var members = GroupHelper.GetGroupMembers(groupName);
            //var list = members.Select(m => m.EmailAddress).ToList();
            //return Result.Ok(list);
            return new Result<List<string>>();
        }
    }
}
