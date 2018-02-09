using System.Data.Entity.ModelConfiguration;
using ADEN.Web.Models;

namespace ADEN.Web.Data.EntityConfigurations
{
    public class ReportDocumentConfiguration : EntityTypeConfiguration<ReportDocument>
    {
        public ReportDocumentConfiguration()
        {
            ToTable("Aden.ReportDocuments");
            Property(s => s.Id).HasColumnName("ReportDocumentId");
            Property(s => s.ReportLevel).HasColumnName("ReportLevelId");
        }
    }
}