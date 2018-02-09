using ADEN.Web.Data;
using ADEN.Web.Helpers;
using ADEN.Web.Models;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace ADEN.Web.Core
{
    public class UnitOfWork
    {
        private readonly AdenContext _context;
        public FileSpecificationRepository FileSpecifications { get; set; }
        public ReportRepository Reports { get; set; }
        public WorkItemRepository WorkItems { get; set; }
        public DocumentRepository Documents { get; set; }

        public UnitOfWork(AdenContext context)
        {
            _context = context;
            FileSpecifications = new FileSpecificationRepository(context);
            Reports = new ReportRepository(context);
            WorkItems = new WorkItemRepository(context);
            Documents = new DocumentRepository(context);
        }

        public OperationResult GenerateDocuments(int reportId)
        {
            var report = _context.Reports.Include(r => r.FileSpecification).Include(r => r.Documents).SingleOrDefault(r => r.Id == reportId);

            if (report == null) return new OperationResult("Unable to generate document. Report not found", false);

            if (report.FileSpecification.IsSCH)
            {
                var dataTable = ExecuteDocumentCreation(report.FileSpecification, "SCH");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.SCH);
            }
            if (report.FileSpecification.IsLEA)
            {

                var dataTable = ExecuteDocumentCreation(report.FileSpecification, "LEA");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.LEA);
            }
            if (report.FileSpecification.IsSEA)
            {
                var dataTable = ExecuteDocumentCreation(report.FileSpecification, "SEA");
                var file = dataTable.ToCSVBytes();
                report.CreateDocument(file, ReportLevel.SEA);
            }

            Complete();

            return new OperationResult("Documents created successfully");
        }

        private DataTable ExecuteDocumentCreation(FileSpecification specification, string reportLevel)
        {
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(_context.Database.Connection.ConnectionString))
            {
                using (var cmd = new SqlCommand(specification.ReportAction, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DataYear", specification.DataYear);
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
    }
}