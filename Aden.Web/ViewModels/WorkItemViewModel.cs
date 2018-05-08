using System;
using System.ComponentModel.DataAnnotations;
using Aden.Core.Helpers;
using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace Aden.Web.ViewModels
{
    public class WorkItemViewModel : IMapFrom<WorkItem>, IHaveCustomMappings
    {
        public WorkItemViewModel()
        {
            //Files = new List<HttpPostedFileBase>();
        }

        public int Id { get; set; }
        public int ReportId { get; set; }
        public int DataYear { get; set; }

        [Display(Name = "Data Year")]
        public string DisplayDataYear
        {
            get { return string.Format("{0}-{1}", DataYear - 1, DataYear); }
        }

        public DateTime? AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string State { get; set; }
        public string Action { get; set; }
        public string ReportAction { get; set; }

        public WorkItemAction WorkItemActionId { get; set; }

        public string FileName { get; set; }
        public string FileNumber { get; set; }

        public bool CanCancel { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool IsManualUpload
        {
            get { return ReportAction == "Manual"; }
        }


        [Required]
        public string Notes { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<WorkItem, WorkItemViewModel>()
                .ForMember(d => d.DataYear, opt => opt.MapFrom(s => s.Report.Submission.DataYear))
                .ForMember(d => d.DueDate, opt => opt.MapFrom(s => s.Report.Submission.DueDate))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.WorkItemState.GetDisplayName()))
                .ForMember(d => d.CompletedDate, opt => opt.MapFrom(s => s.CompletedDate))
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileNumber))
                .ForMember(d => d.WorkItemActionId, opt => opt.MapFrom(s => s.WorkItemAction))
                .ForMember(d => d.ReportAction, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.ReportAction))
                .ForMember(d => d.Action, opt => opt.MapFrom(s => s.WorkItemAction.GetDisplayName()));
        }
    }
}