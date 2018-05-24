using System.Collections.Generic;
using System.Threading.Tasks;
using Aden.Core.Models;

namespace Aden.Core.Repositories
{
    public interface IReportRepository
    {
        IEnumerable<Report> GetByFileSpecification(int datayear, string fileNumber = "");
        Task<IEnumerable<Report>> GetByFileSpecificationAsync(int datayear, string fileNumber = "");
        IEnumerable<Report> GetByFileSpecificationNumberPaged(string search, string order, int offset, int limit);
    }
}