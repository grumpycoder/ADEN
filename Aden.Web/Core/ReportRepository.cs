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

        public IEnumerable<Report> GetByFileSpecificationNumber(string fileNumber = null)
        {
            return _context.Reports.Include(f => f.FileSpecification).Include(r => r.Documents)
                .Where(f => (f.FileSpecification.FileNumber == fileNumber && f.FileSpecification.DataYear == f.DataYear) || string.IsNullOrEmpty(fileNumber)).ToList();
        }

    }
}