using System.Data.Entity.ModelConfiguration;
using ADEN.Web.Models;

namespace ADEN.Web.Data.EntityConfigurations
{
    public class ReportConfiguration : EntityTypeConfiguration<Report>
    {
        public ReportConfiguration()
        {
            ToTable("Aden.Reports");
            Property(s => s.Id).HasColumnName("ReportId");
            Property(s => s.ReportState).HasColumnName("ReportStateId");
            Property(s => s.SubmittedUser).HasMaxLength(75);
            Property(s => s.ApprovedUser).HasMaxLength(75);
            Property(s => s.SubmittedUser).HasMaxLength(75);

        }
    }
}