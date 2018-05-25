using Aden.Core.Data;
using Aden.Core.Helpers;
using Aden.Core.Models;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Aden.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdenContext _context;
        public IFileSpecificationRepository FileSpecifications { get; set; }
        public IReportRepository Reports { get; set; }
        public IWorkItemRepository WorkItems { get; set; }
        public IDocumentRepository Documents { get; set; }
        public ISubmissionRepository Submissions { get; set; }

        public UnitOfWork(AdenContext context, IFileSpecificationRepository fileSpecificationRepository,
            IReportRepository reportRepository, ISubmissionRepository submissionRepository,
            IWorkItemRepository workItemRepository, IDocumentRepository documentRepository)
        {
            _context = context;
            FileSpecifications = fileSpecificationRepository;
            Reports = reportRepository;
            Submissions = submissionRepository;
            WorkItems = workItemRepository;
            Documents = documentRepository;
        }


        public UnitOfWork()
        {
            _context = new AdenContext();
            FileSpecifications = new FileSpecificationRepository(_context);
            Reports = new ReportRepository(_context);
            Submissions = new SubmissionRepository(_context);
            WorkItems = new WorkItemRepository(_context);
            Documents = new DocumentRepository(_context);
        }

        public async Task<OperationResult> GenerateDocumentsAsync(int reportId)
        {
            var report = await _context.Reports.Include(r => r.Submission).Include(r => r.Documents).SingleOrDefaultAsync(r => r.Id == reportId);

            if (report == null) return new OperationResult("Unable to generate document. Report not found", false);

            if (report.Submission.IsSCH)
            {
                var dataTable = ExecuteDocumentCreation(report.Submission, "SCH");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.SCH);
            }
            if (report.Submission.IsLEA)
            {
                var dataTable = ExecuteDocumentCreation(report.Submission, "LEA");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.LEA);
            }
            if (report.Submission.IsSEA)
            {
                var dataTable = ExecuteDocumentCreation(report.Submission, "SEA");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.SEA);
            }

            await CompleteAsync();

            return new OperationResult("Documents created successfully");
        }

        public OperationResult GenerateDocuments(int reportId)
        {
            var report = _context.Reports.Include(r => r.Submission).Include(r => r.Documents).SingleOrDefault(r => r.Id == reportId);

            if (report == null) return new OperationResult("Unable to generate document. Report not found", false);

            if (report.Submission.IsSCH)
            {
                var dataTable = ExecuteDocumentCreation(report.Submission, "SCH");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.SCH);
            }
            if (report.Submission.IsLEA)
            {

                var dataTable = ExecuteDocumentCreation(report.Submission, "LEA");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.LEA);
            }
            if (report.Submission.IsSEA)
            {
                var dataTable = ExecuteDocumentCreation(report.Submission, "SEA");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.SEA);
            }

            Complete();

            return new OperationResult("Documents created successfully");
        }

        private DataTable ExecuteDocumentCreation(Submission submission, string reportLevel)
        {
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_context.Database.Connection.ConnectionString))
            {
                using (var cmd = new SqlCommand(submission.FileSpecification.ReportAction, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DataYear", submission.DataYear);
                    cmd.Parameters.AddWithValue("@ReportLevel", reportLevel);
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public void Complete()
        {
            _context.SaveChanges();
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}