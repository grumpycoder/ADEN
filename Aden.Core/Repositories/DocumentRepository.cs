using Aden.Core.Data;
using Aden.Core.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Aden.Core.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AdenContext _context;

        public DocumentRepository(AdenContext context)
        {
            _context = context;
        }

        public async Task<ReportDocument> GetByIdAsync(int id)
        {
            return await _context.ReportDocuments.SingleOrDefaultAsync(r => r.Id == id);
        }

        public ReportDocument GetById(int id)
        {
            return _context.ReportDocuments.SingleOrDefault(r => r.Id == id);
        }

        public async Task<IEnumerable<ReportDocument>> GetBySubmissionIdAsync(int submissionId)
        {
            return await _context.ReportDocuments.Where(d => d.Report.SubmissionId == submissionId).ToListAsync();
        }

        public async Task<int> GetNextAvailableVersion(int submissionId, ReportLevel reportLevel)
        {
            var version = 0;
            var hasDocuments = await _context.ReportDocuments.AnyAsync(d => d.Report.SubmissionId == submissionId && d.ReportLevel == reportLevel);

            if (hasDocuments) version = await _context.ReportDocuments.Where(d => d.Report.SubmissionId == submissionId && d.ReportLevel == reportLevel).MaxAsync(x => x.Version);

            return version + 1;
        }

        public IEnumerable<ReportDocument> GetBySubmissionId(int submissionId)
        {
            return _context.ReportDocuments.Where(d => d.Report.SubmissionId == submissionId).ToList();
        }

        public void DeleteReportDocuments(int reportId)
        {
            var documents = _context.ReportDocuments.Where(r => r.ReportId == reportId).ToList();
            _context.ReportDocuments.RemoveRange(documents);
        }


    }
}