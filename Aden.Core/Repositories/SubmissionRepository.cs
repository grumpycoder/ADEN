using Aden.Core.Data;
using Aden.Core.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Aden.Core.Repositories
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly AdenContext _context;

        public SubmissionRepository(AdenContext context)
        {
            _context = context;
        }

        public async Task<Submission> GetByIdAsync(int id)
        {
            return await _context.Submissions.Include(s => s.FileSpecification).SingleOrDefaultAsync(x => x.Id == id);
        }

        public Submission GetById(int id)
        {
            return _context.Submissions.Include(s => s.FileSpecification).SingleOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<Submission>> GetBySectionWithReportsAsync(string section = null, string search = null, string order = null, int offset = 0, int limit = 0)
        {
            var submissions = _context.Submissions
                .Where(x => (string.IsNullOrEmpty(section) || x.FileSpecification.Section == section) && (string.IsNullOrEmpty(search)) || (x.FileSpecification.FileName.Contains(search) || x.FileSpecification.FileNumber.Contains(search) || x.FileSpecification.FileNumber.Contains(search)))
                .Include(r => r.Reports).Include(r => r.FileSpecification)
                .OrderBy(x => x.DueDate).ThenByDescending(x => x.Id).Skip(offset).AsQueryable();
            if (limit > 0) submissions = submissions.Take(limit);

            return await submissions.ToListAsync();
        }

        public async Task<IEnumerable<Submission>> GetWithReportsPagedAsync(string search = null, string order = null, int offset = 0, int limit = 0)
        {
            var submissions = _context.Submissions
                .Where(x => (string.IsNullOrEmpty(search)) || (x.FileSpecification.FileName.Contains(search) || x.FileSpecification.FileNumber.Contains(search) || x.FileSpecification.FileNumber.Contains(search)))
                .Include(r => r.Reports).Include(r => r.FileSpecification)
                .OrderBy(x => x.DueDate).ThenByDescending(x => x.Id).Skip(offset).AsQueryable();
            if (limit > 0) submissions = submissions.Take(limit);

            return await submissions.ToListAsync();
        }

        public IEnumerable<Submission> GetWithReportsPaged(string search = null, string order = null, int offset = 0, int limit = 0)
        {
            var submissions = _context.Submissions
                .Where(x => (string.IsNullOrEmpty(search)) || (x.FileSpecification.FileName.Contains(search) || x.FileSpecification.FileNumber.Contains(search) || x.FileSpecification.FileNumber.Contains(search)))
                .Include(r => r.Reports).Include(r => r.FileSpecification)
                .OrderBy(x => x.DueDate).ThenByDescending(x => x.Id).Skip(offset).AsQueryable();
            if (limit > 0) submissions = submissions.Take(limit);

            return submissions.ToList();
        }

        public async Task DeleteAsync(int fileSpecificationId)
        {
            var docs = await _context.ReportDocuments.Where(d =>
                d.Report.ReportState < ReportState.CompleteWithError && d.Report.Submission.FileSpecificationId == fileSpecificationId).DeleteAsync();

            var wi = await _context.WorkItems.Where(w =>
                w.Report.Submission.FileSpecificationId == fileSpecificationId &&
                w.Report.ReportState < ReportState.CompleteWithError).DeleteAsync();

            var reports = await _context.Reports.Where(r =>
                r.Submission.FileSpecificationId == fileSpecificationId &&
                r.Submission.SubmissionState < SubmissionState.CompleteWithError).DeleteAsync();

            var submissions = await _context.Submissions.Where(s =>
                s.FileSpecificationId == fileSpecificationId && s.SubmissionState < SubmissionState.CompleteWithError).DeleteAsync();

        }

        public void Delete(int fileSpecificationId)
        {
            //TODO: Need a check on deleting if active work item
            //TODO: Only delete records in active data year
            var docs = _context.ReportDocuments.Where(d =>
                d.Report.ReportState < ReportState.CompleteWithError && d.Report.Submission.FileSpecificationId == fileSpecificationId).Delete();

            var wi = _context.WorkItems.Where(w =>
                w.Report.Submission.FileSpecificationId == fileSpecificationId &&
                w.Report.ReportState < ReportState.CompleteWithError).Delete();

            var reports = _context.Reports.Where(r =>
                r.Submission.FileSpecificationId == fileSpecificationId &&
                r.Submission.SubmissionState < SubmissionState.CompleteWithError).Delete();

            var submissions = _context.Submissions.Where(s =>
                s.FileSpecificationId == fileSpecificationId && s.SubmissionState < SubmissionState.CompleteWithError).Delete();

        }
    }
}