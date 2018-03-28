using System.Data.Entity.ModelConfiguration;
using Aden.Core.Models;

namespace Aden.Core.Data.EntityConfigurations
{
    public class FileSpecificationConfiguration : EntityTypeConfiguration<FileSpecification>
    {
        public FileSpecificationConfiguration()
        {
            ToTable("Aden.FileSpecifications");
            Property(s => s.Id).HasColumnName("FileSpecificationId");
            Property(s => s.FileNumber).HasMaxLength(8);
            Property(s => s.FileName).HasMaxLength(250);
            //Property(s => s.FileNameFormat).HasMaxLength(50);
            //Property(s => s.DueDate).HasColumnType("datetime2");

        }
    }
}