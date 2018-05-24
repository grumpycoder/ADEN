using Aden.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aden.Core.Repositories
{
    public interface IFileSpecificationRepository
    {
        IEnumerable<FileSpecification> GetAll();
        Task<IEnumerable<FileSpecification>> GetAllAsync();
        FileSpecification GetById(int id);
        Task<FileSpecification> GetByIdAsync(int id);
    }
}