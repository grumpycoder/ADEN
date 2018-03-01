﻿using System;
using System.Collections.Generic;

namespace ADEN.Web.Models
{
    public class FileSpecification
    {
        public int Id { get; set; }
        public string FileNumber { get; set; }
        public string FileName { get; set; }
        public bool? IsRetired { get; set; }
        public string FileNameFormat { get; set; }
        public string ReportAction { get; set; }

        public string Department { get; set; }
        public string GenerationUserGroup { get; set; }
        public string ApprovalUserGroup { get; set; }
        public string SubmissionUserGroup { get; set; }

        public virtual List<Submission> Submissions { get; set; }

        //public bool IsSEA { get; set; }
        //public bool IsLEA { get; set; }
        //public bool IsSCH { get; set; }
        //public DateTime? DueDate { get; set; }
        //public int? DataYear { get; set; }
        //public ReportState ReportState { get; set; }

        //public virtual List<Report> Reports { get; set; }

        public void AddReport(Report report)
        {
            //report.FileSpecification = this;
            //Reports.Add(report);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", FileNumber, FileName);
        }

        public void Reset(DateTime dueDate, int dataYear)
        {
            //DataYear = dataYear;
            //DueDate = dueDate;
            //ReportState = ReportState.NotStarted;
        }
    }
}
