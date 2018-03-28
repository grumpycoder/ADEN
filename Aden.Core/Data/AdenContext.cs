using System;
using System.Data.Entity;
using System.Diagnostics;
using Aden.Core.Data.EntityConfigurations;
using Aden.Core.Models;

namespace Aden.Core.Data
{
    public class AdenContext : DbContext
    {
        public AdenContext()
            : base("AdenContext")
        {
            Database.Log = msg => Debug.WriteLine(msg);
            Database.SetInitializer<AdenContext>(null);
        }

        public static AdenContext Create()
        {
            return new AdenContext();
        }

        public DbSet<Report> Reports { get; set; }
        public DbSet<FileSpecification> FileSpecifications { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<ReportDocument> ReportDocuments { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Properties<string>().Configure(c => c.HasColumnType("varchar").HasMaxLength(255));
            modelBuilder.Properties<string>();

            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("smalldatetime"));


            modelBuilder.Configurations.Add(new ReportConfiguration());
            modelBuilder.Configurations.Add(new FileSpecificationConfiguration());
            modelBuilder.Configurations.Add(new SubmissionConfiguration());
            modelBuilder.Configurations.Add(new WorkItemConfiguration());
            modelBuilder.Configurations.Add(new ReportDocumentConfiguration());
        }
    }
}