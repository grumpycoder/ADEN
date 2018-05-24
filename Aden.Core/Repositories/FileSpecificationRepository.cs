using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Aden.Core.Data;
using Aden.Core.Models;

namespace Aden.Core.Repositories
{
    public class FileSpecificationRepository : IFileSpecificationRepository
    {
        private readonly AdenContext _context;

        public FileSpecificationRepository(AdenContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FileSpecification>> GetAllAsync()
        {
            return await _context.FileSpecifications.ToListAsync();
        }

        public IEnumerable<FileSpecification> GetAll()
        {
            var specs = _context.FileSpecifications.ToList();
            return specs.ToList();
        }

        public async Task<FileSpecification> GetByIdAsync(int id)
        {
            return await _context.FileSpecifications.SingleOrDefaultAsync(x => x.Id == id);
        }

        public FileSpecification GetById(int id)
        {
            return _context.FileSpecifications.SingleOrDefault(x => x.Id == id);
        }

    }
}