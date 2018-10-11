using Aden.Core.Models;
using System.Data.Entity.ModelConfiguration;

namespace Aden.Core.Data.EntityConfigurations
{
    public class SubmissionAuditConfiguration : EntityTypeConfiguration<SubmissionAudit>
    {
        public SubmissionAuditConfiguration()
        {
            ToTable("Aden.SubmissionAudits");
            Property(s => s.Id).HasColumnName("SubmissionAuditId");
        }
    }
}