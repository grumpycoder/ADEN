using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Aden.Core.Data;
using Aden.Core.Models;

namespace Aden.Core.Repositories
{
    public class DocumentRepository
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


        public IEnumerable<ReportDocument> GetByReportId(int id)
        {
            return _context.ReportDocuments.Where(r => r.ReportId == id).ToList();
        }

        public void DeleteReportDocuments(int reportId)
        {
            var documents = _context.ReportDocuments.Where(r => r.ReportId == reportId).ToList();
            _context.ReportDocuments.RemoveRange(documents);
        }
    }
}