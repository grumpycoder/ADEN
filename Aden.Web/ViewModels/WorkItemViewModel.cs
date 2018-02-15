using System;
using ADEN.Web.Helpers;
using ADEN.Web.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace ADEN.Web.ViewModels
{
    public class WorkItemViewModel : IMapFrom<WorkItem>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int DataYear { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string State { get; set; }
        public string Action { get; set; }

        public string FileSpecification { get; set; }

        public bool CanCancel { get; set; }
        public DateTime? CompletedDate { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<WorkItem, WorkItemViewModel>()
                .ForMember(d => d.DataYear, opt => opt.MapFrom(s => s.Report.FileSpecification.DataYear))
                .ForMember(d => d.DueDate, opt => opt.MapFrom(s => s.Report.FileSpecification.DueDate))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.WorkItemState.GetDisplayName()))
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.CompletedDate))
                .ForMember(d => d.FileSpecification, opt => opt.MapFrom(s => s.Report.FileSpecification.FileName))
                .ForMember(d => d.Action, opt => opt.MapFrom(s => s.WorkItemAction.GetDisplayName()));
        }
    }
}