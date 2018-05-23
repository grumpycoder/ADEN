using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Aden.Core.Data;
using Aden.Core.Helpers;
using Aden.Core.Models;

namespace Aden.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdenContext _context;
        public FileSpecificationRepository FileSpecifications { get; set; }
        public ReportRepository Reports { get; set; }
        public WorkItemRepository WorkItems { get; set; }
        public DocumentRepository Documents { get; set; }
        public SubmissionRepository Submissions { get; set; }

        public UnitOfWork(AdenContext context)
        {
            _context = context;
            FileSpecifications = new FileSpecificationRepository(context);
            Reports = new ReportRepository(context);
            WorkItems = new WorkItemRepository(context);
            Documents = new DocumentRepository(context);
            Submissions = new SubmissionRepository(context);
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