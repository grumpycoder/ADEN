using Aden.Core.Data;
using Aden.Core.Helpers;
using Aden.Core.Models;
using CSharpFunctionalExtensions;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Aden.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        //private readonly AdenContext _context;
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

        public async Task<Result> GenerateDocumentsAsync(int reportId)
        {
            var report = await _context.Reports.Include(r => r.Submission).Include(r => r.Documents).SingleOrDefaultAsync(r => r.Id == reportId);

            if (report == null) return Result.Fail("Report not found to generate document");

            if (report.Submission.IsSCH)
            {
                var file = ExecuteDocumentCreationToFile(report, ReportLevel.SCH);
                report.CreateDocument(file, ReportLevel.SCH);
            }
            if (report.Submission.IsLEA)
            {
                var file = ExecuteDocumentCreationToFile(report, ReportLevel.LEA);
                report.CreateDocument(file, ReportLevel.LEA);
            }
            if (report.Submission.IsSEA)
            {
                var file = ExecuteDocumentCreationToFile(report, ReportLevel.SEA);
                report.CreateDocument(file, ReportLevel.SEA);
            }

            await CompleteAsync();

            return Result.Ok($"Document created for {report.Submission.FileSpecification.FileName}");
        }

        private byte[] ExecuteDocumentCreationToFile(Report report, ReportLevel reportLevel)
        {
            var dataTable = new DataTable();
            var ds = new DataSet();
            using (var connection = new SqlConnection(_context.Database.Connection.ConnectionString))
            {
                using (var cmd = new SqlCommand(report.Submission.FileSpecification.ReportAction, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DataYear", report.Submission.DataYear);
                    cmd.Parameters.AddWithValue("@ReportLevel", reportLevel);
                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                    adapter.Fill(ds);
                }
            }

            var version = report.GetNextFileVersionNumber(reportLevel);
            var filename = report.Submission.FileSpecification.FileNameFormat.Replace("{level}", reportLevel.GetDisplayName()).Replace("{version}", string.Format("v{0}.csv", version));

            var table1 = ds.Tables[0].UpdateFieldValue("Filename", filename).ToCsvString(false);
            var table2 = ds.Tables[1].UpdateFieldValue("Filename", filename).ToCsvString(false);


            var file = Encoding.ASCII.GetBytes(ds.Tables[0].Rows.Count > 1 ? string.Concat(table2, table1) : string.Concat(table1, table2)); ;

            return file;

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