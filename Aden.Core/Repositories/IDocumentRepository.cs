using Aden.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aden.Core.Repositories
{
    public interface IDocumentRepository
    {
        void DeleteReportDocuments(int reportId);
        ReportDocument GetById(int id);
        Task<ReportDocument> GetByIdAsync(int id);
        IEnumerable<ReportDocument> GetBySubmissionId(int submissionId);
        Task<IEnumerable<ReportDocument>> GetBySubmissionIdAsync(int submissionId);
        Task<int> GetNextAvailableVersion(int submissionId, ReportLevel reportLevel);
    }
}