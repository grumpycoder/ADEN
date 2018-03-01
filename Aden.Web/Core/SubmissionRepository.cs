using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ADEN.Web.Data;
using ADEN.Web.Models;
using Z.EntityFramework.Plus;

namespace ADEN.Web.Core
{
    public class SubmissionRepository
    {
        private readonly AdenContext _context;

        public SubmissionRepository(AdenContext context)
        {
            _context = context;
        }

        public IEnumerable<Submission> GetAllWithReportsPaged(string search = null, string order = null, int offset = 0, int limit = 0)
        {
            var submissions = _context.Submissions
                .Where(x => (string.IsNullOrEmpty(search)) || (x.FileSpecification.FileName.Contains(search) || x.FileSpecification.FileNumber.Contains(search) || x.FileSpecification.FileNumber.Contains(search)))
                .Include(r => r.Reports).Include(r => r.FileSpecification)
                .OrderBy(x => x.Id).Skip(offset).AsQueryable();
            if (limit > 0) submissions = submissions.Take(limit);

            return submissions.ToList();
        }

        public IEnumerable<Submission> GetAllWithReports()
        {
            var submissions = _context.Submissions.Include(r => r.Reports).IncludeFilter(r => r.Reports.Where(x => x.DataYear == r.DataYear)).ToList();
            return submissions.ToList();
        }
    }
}