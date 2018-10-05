using Aden.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aden.Core.Repositories
{
    public interface IReportRepository
    {
        IEnumerable<Report> GetByFileSpecification(int datayear, string fileNumber = "");
        Task<IEnumerable<Report>> GetByFileSpecificationAsync(int datayear, string fileNumber = "");
        IEnumerable<Report> GetByFileSpecificationNumberPaged(string search, string order, int offset, int limit);
        Report GetById(int id);
        Task<Report> GetByIdAsync(int id);
        void RemoveNotStartedWorkItems(int reportId);
    }
}