using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Aden.Core.Data;
using Aden.Core.Models;

namespace Aden.Core.Repositories
{
    public class WorkItemRepository
    {
        private readonly AdenContext _context;

        public WorkItemRepository(AdenContext context)
        {
            _context = context;
        }

        public IEnumerable<WorkItem> GetAll()
        {
            return _context.WorkItems.ToList();
        }

        public IEnumerable<WorkItem> GetByUser(string username)
        {
            return _context.WorkItems
                            .Include(f => f.Report.Submission)
                            .Where(u => u.AssignedUser == username).ToList();
        }

        public WorkItem GetById(int id)
        {
            return _context.WorkItems.Include(r => r.Report.Submission).Include(r => r.Report.WorkItems).SingleOrDefault(w => w.Id == id);
        }

        public WorkItem GetByIdWithDetails(int id)
        {
            return _context.WorkItems.Include(r => r.Report.Submission).Include(r => r.Report.Documents).Include(r => r.Report.WorkItems).SingleOrDefault(w => w.Id == id);
        }

        public IEnumerable<WorkItem> GetActiveByUser(string username)
        {
            return _context.WorkItems
                .Include(f => f.Report.Submission)
                .Where(u => u.AssignedUser == username && u.WorkItemState == WorkItemState.NotStarted).OrderBy(d => d.AssignedDate).ToList();
        }

        public IEnumerable<WorkItem> GetCompletedByUser(string username)
        {
            var workItems = _context.WorkItems
                .Include(f => f.Report.Submission).Include(r => r.Report.WorkItems).AsQueryable();

            return workItems.Where(u => u.AssignedUser == username && u.WorkItemState == WorkItemState.Completed).OrderBy(d => d.AssignedDate).ToList();
        }

        public IEnumerable<WorkItem> GetHistoryByFileSpecification(int submissionId, int dataYear)
        {
            return _context.WorkItems.Where(w => w.Report.SubmissionId == submissionId && w.Report.DataYear == dataYear).OrderByDescending(d => d.Id).ToList();
        }

        public IEnumerable<WorkItem> GetHistoryByReport(int reportId)
        {
            return _context.WorkItems.Where(w => w.ReportId == reportId).OrderByDescending(d => d.Id).ToList();
        }

        public string GetUserWithLeastAssignments(IEnumerable<string> members)
        {
            var assignee = _context.WorkItems.Where(u => members.Contains(u.AssignedUser)).GroupBy(u => u.AssignedUser).Select(n => new
            {
                n.Key,
                Count = n.Count()
            }).OrderBy(x => x.Count).FirstOrDefault();

            return assignee != null ? assignee.Key : string.Empty;
        }
    }
}