using ADEN.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ADEN.Web.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int? DataYear { get; set; }
        public int? SubmittedVersion { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string GeneratedUser { get; set; }
        public string ApprovedUser { get; set; }
        public string SubmittedUser { get; set; }

        public List<ReportDocument> Documents { get; set; }

        public int FileSpecificationId { get; set; }
        public virtual FileSpecification FileSpecification { get; set; }

        public ReportState ReportState { set; get; }

        public List<WorkItem> WorkItems { set; get; }

        public Report()
        {
            WorkItems = new List<WorkItem>();
            Documents = new List<ReportDocument>();
        }

        public static Report Create(FileSpecification spec)
        {
            var report = new Report { DataYear = spec.DataYear, ReportState = ReportState.NotStarted };
            return report;
        }

        public void AddWorkItem(WorkItem workItem)
        {
            WorkItems.Add(workItem);
        }

        public void CreateDocument(byte[] file, ReportLevel reportLevel)
        {
            var version = 0;
            if (Documents.Any(d => d.ReportLevel == reportLevel)) version = Documents.Max(x => x.Version);

            version += 1;
            var filename = FileSpecification.FileNameFormat.Replace("{level}", reportLevel.GetDisplayName()).Replace("{version}", string.Format("v{0}.csv", version));

            var doc = ReportDocument.Create(filename, version, reportLevel, file);
            Documents.Add(doc);
        }

        public void CancelWorkItems()
        {
            foreach (var workItem in WorkItems.Where(i => i.WorkItemState == WorkItemState.NotStarted))
            {
                workItem.Cancel();
            }

        }

        public void StartNewWork()
        {
            var wi = WorkItem.Create(WorkItemAction.Generate, FileSpecification.GenerationUserGroup);
            AddWorkItem(wi);
            ReportState = FileSpecification.ReportState = ReportState.AssignedForGeneration;

        }

        public void Waive()
        {
            ReportState = FileSpecification.ReportState = ReportState.Waived;
        }
    }
}
