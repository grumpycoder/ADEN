using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Aden.Core.Data;
using Aden.Core.Models;
using Z.EntityFramework.Plus;

namespace Aden.Core.Repositories
{
    public class SubmissionRepository
    {
        private readonly AdenContext _context;

        public SubmissionRepository(AdenContext context)
        {
            _context = context;
        }

        public IEnumerable<Submission> GetAllBySectionWithReportsPaged(string section, string search = null, string order = null, int offset = 0, int limit = 0)
        {
            var submissions = _context.Submissions
                .Where(x => (string.IsNullOrEmpty(section) || x.FileSpecification.Section == section) && (string.IsNullOrEmpty(search)) || (x.FileSpecification.FileName.Contains(search) || x.FileSpecification.FileNumber.Contains(search) || x.FileSpecification.FileNumber.Contains(search)))
                .Include(r => r.Reports).Include(r => r.FileSpecification)
                .OrderBy(x => x.DueDate).ThenByDescending(x => x.Id).Skip(offset).AsQueryable();
            if (limit > 0) submissions = submissions.Take(limit);

            return submissions.ToList();
        }

        public IEnumerable<Submission> GetAllWithReportsPaged(string search = null, string order = null, int offset = 0, int limit = 0)
        {
            var submissions = _context.Submissions
                .Where(x => (string.IsNullOrEmpty(search)) || (x.FileSpecification.FileName.Contains(search) || x.FileSpecification.FileNumber.Contains(search) || x.FileSpecification.FileNumber.Contains(search)))
                .Include(r => r.Reports).Include(r => r.FileSpecification)
                .OrderBy(x => x.DueDate).ThenByDescending(x => x.Id).Skip(offset).AsQueryable();
            if (limit > 0) submissions = submissions.Take(limit);

            return submissions.ToList();
        }

        public IEnumerable<Submission> GetAllWithReports()
        {
            var submissions = _context.Submissions.Include(r => r.Reports).IncludeFilter(r => r.Reports.Where(x => x.DataYear == r.DataYear)).ToList();
            return submissions.ToList();
        }

        public Submission GetById(int id)
        {
            return _context.Submissions.Include(s => s.FileSpecification).SingleOrDefault(x => x.Id == id);
        }

        public void Delete(int fileSpecificationId)
        {
            var docs = _context.ReportDocuments.Where(d =>
                d.Report.ReportState < ReportState.CompleteWithError && d.Report.Submission.FileSpecificationId == fileSpecificationId).Delete();

            var wi = _context.WorkItems.Where(w =>
                w.Report.Submission.FileSpecificationId == fileSpecificationId &&
                w.Report.ReportState < ReportState.CompleteWithError).Delete();

            var reports = _context.Reports.Where(r =>
                r.Submission.FileSpecificationId == fileSpecificationId &&
                r.Submission.ReportState < ReportState.CompleteWithError).Delete();

            var submissions = _context.Submissions.Where(s =>
                s.FileSpecificationId == fileSpecificationId && s.ReportState < ReportState.CompleteWithError).Delete();

        }
    }
}