using ALSDE.Idem;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Aden.Core.Services
{
    public interface IMembershipService
    {
        Result<List<string>> GetGroupMembers(string groupName);
    }

    public class MembershipService : IMembershipService
    {

        public MembershipService()
        {

        }

        public Result<List<string>> GetGroupMembers(string groupName)
        {
            var members = GroupHelper.GetGroupMembers(groupName);
            var list = members.Select(m => m.EmailAddress).ToList();
            return Result.Ok(list);
        }
    }
}
