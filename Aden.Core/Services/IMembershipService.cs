using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace Aden.Core.Services
{
    public interface IMembershipService
    {
        Result<List<string>> GetGroupMembers(string groupName);
    }
}