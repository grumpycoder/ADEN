using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ADEN.Web.Data;
using ADEN.Web.Models;
using Z.EntityFramework.Plus;

namespace ADEN.Web.Core
{
    public class FileSpecificationRepository
    {
        private readonly AdenContext _context;

        public FileSpecificationRepository(AdenContext context)
        {
            _context = context;
        }

        public IEnumerable<FileSpecification> GetAll()
        {
            //return _context.FileSpecifications.OrderBy(d => d.DueDate).ToList();
            return _context.FileSpecifications.ToList();
        }

        public FileSpecification GetById(int id)
        {
            return _context.FileSpecifications.SingleOrDefault(x => x.Id == id);
        }

        //public IEnumerable<FileSpecification> GetAllWithAllReports()
        //{
        //    return _context.FileSpecifications.Include(r => r.Reports).OrderBy(d => d.DueDate).ToList();
        //}

        public IEnumerable<FileSpecification> GetAllWithReports()
        {
            //var specs = _context.FileSpecifications.Include(r => r.Reports).IncludeFilter(r => r.Reports.Where(x => x.DataYear == r.DataYear)).ToList();
            var specs = _context.FileSpecifications.ToList();
            return specs.ToList();
        }

        public FileSpecification GetByNumber(string fileNumber)
        {
            return _context.FileSpecifications.SingleOrDefault(f => f.FileNumber == fileNumber);
        }

        public IEnumerable<FileSpecification> GetAllWithReportsPaged(string search = null, string order = null, int offset = 0, int limit = 0)
        {
            //var specs = _context.FileSpecifications
            //        .Where(x => (string.IsNullOrEmpty(search)) || (x.FileName.Contains(search) || x.FileNumber.Contains(search) || x.FileNumber.Contains(search)))
            //        .Include(r => r.Reports)
            //        .IncludeFilter(r => r.Reports.Where(x => x.DataYear == r.DataYear))
            //        .OrderBy(x => x.Id).Skip(offset).AsQueryable();
            var specs = _context.FileSpecifications
                .Where(x => (string.IsNullOrEmpty(search)) || (x.FileName.Contains(search) || x.FileNumber.Contains(search) || x.FileNumber.Contains(search)))
                .Include(r => r.Submissions)
                .OrderBy(x => x.Id).Skip(offset).AsQueryable();
            if (limit > 0) specs = specs.Take(limit);

            return specs.ToList();
        }
    }
}