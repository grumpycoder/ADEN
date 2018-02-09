using System.Data.Entity.ModelConfiguration;
using ADEN.Web.Models;

namespace ADEN.Web.Data.EntityConfigurations
{
    public class WorkItemConfiguration : EntityTypeConfiguration<WorkItem>
    {
        public WorkItemConfiguration()
        {
            ToTable("Aden.WorkItems");
            Property(s => s.Id).HasColumnName("WorkItemId");
            Property(s => s.WorkItemAction).HasColumnName("WorkItemActionId");
            Property(s => s.WorkItemState).HasColumnName("WorkItemStateId");
            Property(s => s.AssignedUser).HasMaxLength(75);
            Property(s => s.AssignedDate).HasColumnType("datetime2");
        }
    }
}