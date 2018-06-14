using Aden.Core.Repositories;
using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace Aden.Core.Services
{
    public interface IAssignmentService
    {
        Result<string> GetAssigneeFromMemberList(List<string> members);
    }

    public class AssignmentService : IAssignmentService
    {
        private readonly IUnitOfWork _uow;

        public AssignmentService(IUnitOfWork uow)
        {
            _uow = uow;
        }


        public Result<string> GetAssigneeFromMemberList(List<string> members)
        {
            if (members == null || members.Count == 0) return Result.Fail<string>("Member list should not be empty");

            var member = _uow.WorkItems.GetUserWithLeastAssignments(members);

            return Result.Ok(member);
        }
    }
}
