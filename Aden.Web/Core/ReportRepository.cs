using ADEN.Web.Data;
using ADEN.Web.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ADEN.Web.Core
{
    public class ReportRepository
    {
        private readonly AdenContext _context;

        public ReportRepository(AdenContext context)
        {
            _context = context;
        }

        public IEnumerable<Report> GetByFileSpecificationNumber(string fileNumber = null)
        {
            return _context.Reports.Include(f => f.FileSpecification).Include(r => r.Documents)
                .Where(f => (f.FileSpecification.FileNumber == fileNumber && f.FileSpecification.DataYear == f.DataYear) || string.IsNullOrEmpty(fileNumber)).ToList();
        }

        public IEnumerable<Report> GetByFileSpecificationNumberPaged(string search, string order, int offset, int limit)
        {
            var reports = _context.Reports
                .Include(f => f.FileSpecification)
                .Include(r => r.Documents)
                .OrderBy(x => x.Id)
                .Where(f => (string.IsNullOrEmpty(search)) ||
                            (f.FileSpecification.FileNumber.Contains(search) ||
                             f.FileSpecification.FileName.Contains(search)))
                 .Skip(offset).AsQueryable();

            if (limit > 0) reports = reports.Take(limit);

            return reports.ToList();

        }
    }
}