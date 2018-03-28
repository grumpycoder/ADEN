using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<ReportDocument> GetByReportId(int id)
        {
            return _context.ReportDocuments.Where(r => r.ReportId == id).ToList();
        }

        public ReportDocument GetById(int id)
        {
            return _context.ReportDocuments.SingleOrDefault(r => r.Id == id);
        }

        public void DeleteReportDocuments(int reportId)
        {
            var documents = _context.ReportDocuments.Where(r => r.ReportId == reportId).ToList();
            _context.ReportDocuments.RemoveRange(documents);
        }
    }
}