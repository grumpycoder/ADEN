using ADEN.Web.Data;
using ADEN.Web.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
            return _context.FileSpecifications.OrderBy(d => d.DueDate).ToList();
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
            var specs = _context.FileSpecifications.Include(r => r.Reports).IncludeFilter(r => r.Reports.Where(x => x.DataYear == r.DataYear)).ToList();
            return specs;
        }

        public FileSpecification GetByNumber(string fileNumber)
        {
            return _context.FileSpecifications.SingleOrDefault(f => f.FileNumber == fileNumber);
        }
    }
}