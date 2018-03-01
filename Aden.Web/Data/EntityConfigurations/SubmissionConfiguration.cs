using System.Data.Entity.ModelConfiguration;
using ADEN.Web.Models;

namespace ADEN.Web.Data.EntityConfigurations
{
    public class SubmissionConfiguration : EntityTypeConfiguration<Submission>
    {
        public SubmissionConfiguration()
        {
            ToTable("Aden.Submissions");
            Property(s => s.Id).HasColumnName("SubmisssionId");
            Property(s => s.ReportState).HasColumnName("ReportStateId");
        }
    }
}