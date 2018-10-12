using Aden.Core.Models;
using System.Data.Entity.ModelConfiguration;

namespace Aden.Core.Data.EntityConfigurations
{
    public class WorkItemImageConfiguration : EntityTypeConfiguration<WorkItemImage>
    {
        public WorkItemImageConfiguration()
        {
            ToTable("Aden.WorkItemImages");
            Property(s => s.Id).HasColumnName("WorkItemImageId");
        }
    }
}