using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ADEN.Web.Data;
using ADEN.Web.Models;

namespace ADEN.Web.Core
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
                            .Include(f => f.Report.FileSpecification)
                            .Where(u => u.AssignedUser == username).ToList();
        }

        public WorkItem GetById(int id)
        {
            return _context.WorkItems.Include(r => r.Report.FileSpecification).Include(r => r.Report.WorkItems).SingleOrDefault(w => w.Id == id);
        }

        public WorkItem GetByIdWithDetails(int id)
        {
            return _context.WorkItems.Include(r => r.Report.FileSpecification).Include(r => r.Report.Documents).Include(r => r.Report.WorkItems).SingleOrDefault(w => w.Id == id);
        }

        public IEnumerable<WorkItem> GetActiveByUser(string username)
        {
            return _context.WorkItems
                .Include(f => f.Report.FileSpecification)
                .Where(u => u.AssignedUser == username && u.WorkItemState == WorkItemState.NotStarted).OrderBy(d => d.AssignedDate).ToList();
        }

        public IEnumerable<WorkItem> GetCompletedByUser(string username)
        {
            var workItems = _context.WorkItems
                .Include(f => f.Report.FileSpecification).Include(r => r.Report.WorkItems).AsQueryable();

            return workItems.Where(u => u.AssignedUser == username && u.WorkItemState == WorkItemState.Completed).OrderBy(d => d.AssignedDate).ToList();
        }

        public IEnumerable<WorkItem> GetHistoryByFileSpecification(int fileSpecificationId, int dataYear)
        {
            return _context.WorkItems.Where(w => w.Report.FileSpecificationId == fileSpecificationId && w.Report.DataYear == dataYear).OrderByDescending(d => d.Id).ToList();
        }

        public IEnumerable<WorkItem> GetHistoryByReport(int reportId)
        {
            return _context.WorkItems.Where(w => w.ReportId == reportId).OrderByDescending(d => d.Id).ToList();
        }
    }
}