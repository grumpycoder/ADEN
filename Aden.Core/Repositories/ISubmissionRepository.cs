using System.Collections.Generic;
using System.Threading.Tasks;
using Aden.Core.Models;

namespace Aden.Core.Repositories
{
    public interface ISubmissionRepository
    {
        void Delete(int fileSpecificationId);
        Task DeleteAsync(int fileSpecificationId);
        Submission GetById(int id);
        Task<Submission> GetByIdAsync(int id);
        Task<IEnumerable<Submission>> GetBySectionWithReportsAsync(string section = null, string search = null, string order = null, int offset = 0, int limit = 0);
        IEnumerable<Submission> GetWithReportsPaged(string search = null, string order = null, int offset = 0, int limit = 0);
        Task<IEnumerable<Submission>> GetWithReportsPagedAsync(string search = null, string order = null, int offset = 0, int limit = 0);
    }
}