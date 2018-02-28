using System.Data.Entity.ModelConfiguration;
using ADEN.Web.Models;

namespace ADEN.Web.Data.EntityConfigurations
{
    public class FileSpecificationConfiguration : EntityTypeConfiguration<FileSpecification>
    {
        public FileSpecificationConfiguration()
        {
            ToTable("Aden.FileSpecifications");
            Property(s => s.Id).HasColumnName("FileSpecificationId");
            Property(s => s.ReportState).HasColumnName("ReportStateId");
            Property(s => s.FileNumber).HasMaxLength(8);
            Property(s => s.FileName).HasMaxLength(250);
            //Property(s => s.FileNameFormat).HasMaxLength(50);
            //Property(s => s.DueDate).HasColumnType("datetime2");

        }
    }
}