using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ADEN.Web.Data;
using ADEN.Web.Models;

namespace ADEN.Web.Core
{
    public class ReportRepository
    {
        private readonly AdenContext _context;

        public ReportRepository(AdenContext context)
        {
            _context = context;
        }

        public IEnumerable<Report> GetByFileSpecificationNumber(string fileNumber, int datayear)
        {
            return _context.Reports.Include(f => f.Submission).Include(r => r.Documents)
                .Where(f => (f.Submission.FileSpecification.FileNumber == fileNumber && f.Submission.DataYear == datayear) || string.IsNullOrEmpty(fileNumber))
                .OrderByDescending(x => x.Id)
                .ToList();
        }


        public IEnumerable<Report> GetByFileSpecificationNumber(string fileNumber)
        {

            return _context.Reports.Include(f => f.Submission).Include(r => r.Documents).OrderByDescending(x => x.Id)
                .Where(f => (f.Submission.FileSpecification.FileNumber == fileNumber) || string.IsNullOrEmpty(fileNumber)).ToList();
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