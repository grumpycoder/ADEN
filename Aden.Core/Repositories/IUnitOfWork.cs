using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace Aden.Core.Repositories
{
    public interface IUnitOfWork
    {
        IDocumentRepository Documents { get; set; }
        IFileSpecificationRepository FileSpecifications { get; set; }
        IReportRepository Reports { get; set; }
        ISubmissionRepository Submissions { get; set; }
        IWorkItemRepository WorkItems { get; set; }

        void Complete();
        Task CompleteAsync();
        Result GenerateDocuments(int reportId);
        Task<OperationResult> GenerateDocumentsAsync(int reportId);
    }
}