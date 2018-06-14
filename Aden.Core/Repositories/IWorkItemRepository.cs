using System.Collections.Generic;
using System.Threading.Tasks;
using Aden.Core.Models;

namespace Aden.Core.Repositories
{
    public interface IWorkItemRepository
    {
        Task<IEnumerable<WorkItem>> GetActiveAsync(string username);
        IEnumerable<WorkItem> GetActiveByUser(string username);
        IEnumerable<WorkItem> GetAll();
        WorkItem GetById(int id);
        Task<WorkItem> GetByIdAsync(int id);
        WorkItem GetByIdWithDetails(int id);
        IEnumerable<WorkItem> GetByUser(string username);
        IEnumerable<WorkItem> GetCompleted(string username);
        Task<IEnumerable<WorkItem>> GetCompletedAsync(string username);
        IEnumerable<WorkItem> GetCompletedByUser(string username);
        IEnumerable<WorkItem> GetHistory(int reportId);
        Task<IEnumerable<WorkItem>> GetHistoryAsync(int reportId);
        string GetUserWithLeastAssignments(IEnumerable<string> members);
    }
}