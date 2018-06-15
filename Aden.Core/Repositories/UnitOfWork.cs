using Aden.Core.Data;
using Aden.Core.Helpers;
using Aden.Core.Models;
using CSharpFunctionalExtensions;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        public async Task<OperationResult> GenerateDocumentsAsync(int reportId)
        {
            var report = await _context.Reports.Include(r => r.Submission).Include(r => r.Documents).SingleOrDefaultAsync(r => r.Id == reportId);

            if (report == null) return new OperationResult("Unable to generate document. Report not found", false);

            if (report.Submission.IsSCH)
            {
                var file = ExecuteDocumentCreationToFile(report.Submission, "SCH");
                report.CreateDocument(file, ReportLevel.SCH);
            }
            if (report.Submission.IsLEA)
            {
                var file = ExecuteDocumentCreationToFile(report.Submission, "LEA");
                report.CreateDocument(file, ReportLevel.LEA);
            }
            if (report.Submission.IsSEA)
            {
                var file = ExecuteDocumentCreationToFile(report.Submission, "SEA");
                report.CreateDocument(file, ReportLevel.SEA);
            }

            await CompleteAsync();

            return new OperationResult("Documents created successfully");
        }

        public Result GenerateDocuments(int reportId)
        {
            var report = _context.Reports.Include(r => r.Submission).Include(r => r.Documents).SingleOrDefault(r => r.Id == reportId);

            //TODO: Error handling creating documents
            if (report == null) return Result.Fail("Unable to generate document. Report not found");

            if (report.Submission.IsSCH)
            {
                var dataTable = ExecuteDocumentCreation(report.Submission, "SCH");
                var file = dataTable.ToCsvBytes();
                report.CreateDocument(file, ReportLevel.SCH);
            }
            if (report.Submission.IsLEA)
            {

                var dataTable = ExecuteDocumentCreation(report.Submission, "LEA");
                var file = dataTable.ToCsvBytes();
                report.CreateDocument(file, ReportLevel.LEA);
            }
            if (report.Submission.IsSEA)
            {
                var dataTable = ExecuteDocumentCreation(report.Submission, "SEA");
                var file = dataTable.ToCsvBytes();
                report.CreateDocument(file, ReportLevel.SEA);
            }

            Complete();

            return Result.Ok();
        }

        private byte[] ExecuteDocumentCreationToFile(Submission submission, string reportLevel)
        {
            var dataTable = new DataTable();
            var ds = new DataSet();
            using (var connection = new SqlConnection(_context.Database.Connection.ConnectionString))
            {
                using (var cmd = new SqlCommand(submission.FileSpecification.ReportAction, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DataYear", submission.DataYear);
                    cmd.Parameters.AddWithValue("@ReportLevel", reportLevel);
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                    adapter.Fill(ds);
                }
            }
            
            var table1 = ds.Tables[0].ToCsvString(false);
            var table2 = ds.Tables[1].ToCsvString(false);

            var file = Encoding.ASCII.GetBytes(ds.Tables[0].Rows.Count > 1 ? string.Concat(table2, table1) : string.Concat(table1, table2)); ;

            return file; 

            //return Encoding.ASCII.GetBytes(ds.Tables[0].Rows.Count > 1 ? string.Concat(table2, table1) : string.Concat(table1, table2));

            //if (ds.Tables[0].Rows.Count > 1)
            //{
            //    return Encoding.ASCII.GetBytes(string.Concat(table2, table1));
            //}
            //return Encoding.ASCII.GetBytes(string.Concat(table1, table2));
        }

        private DataTable ExecuteDocumentCreation(Submission submission, string reportLevel)
        {
            var dataTable = new DataTable();
            var ds = new DataSet();
            using (var connection = new SqlConnection(_context.Database.Connection.ConnectionString))
            {
                using (var cmd = new SqlCommand(submission.FileSpecification.ReportAction, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DataYear", submission.DataYear);
                    cmd.Parameters.AddWithValue("@ReportLevel", reportLevel);
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                    adapter.Fill(ds);
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