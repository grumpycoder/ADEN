//using ALSDE.Idem;
using ALSDE.Idem;
using ALSDE.Services;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Linq;
using ALSDE.Dtos;

namespace Aden.Core.Services
{
    public class IdemMembershipService : IMembershipService
    {
        
        public IdemMembershipService()
        {
        }

        public Result<List<string>> GetGroupMembers(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName)) return Result.Fail<List<string>>("Group name should not be empty");

            var groupService = new IdemGroupService();
            var members = groupService.GetGroupUsers(groupName);


            if (members == null) return Result.Fail<List<string>>($"No members defined in group {groupName}");

            var list = members.Select(m => m.EmailAddress).ToList();
            return Result.Ok(list);
        }

        public Result<List<GroupDto>> GetGroups(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName)) return Result.Fail<List<GroupDto>>("Group name should not be empty");

            var groupService = new IdemGroupService();
           
            var groups = groupService.GetGroups(groupName);

            return Result.Ok(groups);
        }

        public bool GroupExists(string groupName)
        {
            var groupService = new IdemGroupService();
            return groupService.GroupExists(groupName);

        }
    }
}
