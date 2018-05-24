using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Aden.Core.Data;
using Aden.Core.Models;

namespace Aden.Core.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AdenContext _context;

        public ReportRepository(AdenContext context)
        {
            _context = context;
        }

        public IEnumerable<Report> GetByFileSpecification(int datayear, string fileNumber = "")
        {
            return _context.Reports.Include(f => f.Submission.FileSpecification).Include(r => r.Documents)
                .Where(f => (f.Submission.FileSpecification.FileNumber == fileNumber && f.Submission.DataYear == datayear) || string.IsNullOrEmpty(fileNumber))
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public async Task<IEnumerable<Report>> GetByFileSpecificationAsync(int datayear, string fileNumber = "")
        {
            return await _context.Reports.Include(f => f.Submission.FileSpecification).Include(r => r.Documents)
                .Where(f => (f.Submission.FileSpecification.FileNumber == fileNumber && f.Submission.DataYear == datayear) || string.IsNullOrEmpty(fileNumber))
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public IEnumerable<Report> GetByFileSpecificationNumberPaged(string search, string order, int offset, int limit)
        {
            var reports = _context.Reports
                .Include(f => f.Submission)
                .Include(r => r.Documents)
                .OrderByDescending(x => x.Id)
                .Where(f => (string.IsNullOrEmpty(search)) ||
                            (f.Submission.FileSpecification.FileNumber.Contains(search) ||
                             f.Submission.FileSpecification.FileName.Contains(search)))
                 .Skip(offset).AsQueryable();

            if (limit > 0) reports = reports.Take(limit);

            return reports.ToList();

        }
    }
}