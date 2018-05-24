using System.Threading.Tasks;

namespace Aden.Core.Repositories
{
    public interface IUnitOfWork
    {
        DocumentRepository Documents { get; set; }
        IFileSpecificationRepository FileSpecifications { get; set; }
        IReportRepository Reports { get; set; }
        SubmissionRepository Submissions { get; set; }
        WorkItemRepository WorkItems { get; set; }

        void Complete();
        Task CompleteAsync();
        OperationResult GenerateDocuments(int reportId);
        Task<OperationResult> GenerateDocumentsAsync(int reportId);
    }
}