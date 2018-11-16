using ALSDE.Dtos;
using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace Aden.Core.Services
{
    public interface IMembershipService
    {
        Result<List<string>> GetGroupMembers(string groupName);
        Result<List<GroupDto>> GetGroups(string groupName);
        bool GroupExists(string groupName);
    }
}
