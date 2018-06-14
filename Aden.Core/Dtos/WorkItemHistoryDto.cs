using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;
using System;

namespace Aden.Core.Dtos
{
    public class WorkItemHistoryDto : IMapFrom<WorkItem>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string AssignedUser { get; set; }
        public WorkItemState WorkItemState { get; set; }



        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<WorkItem, WorkItemHistoryDto>()
                //.ForMember(d => d.AssignedDate, opt => opt.MapFrom(s => s.AssignedDate.ToString("MM/dd/yyyy hh:mm AA")))
                //.ForMember(d => d.WorkItemState, opt => opt.MapFrom(s => s.WorkItemState.GetDisplayName()))
                //.ForMember(d => d.CompletedDate, opt => opt.MapFrom(s => s.CompletedDate))
                //.ForMember(d => d.Action, opt => opt.MapFrom(s => s.WorkItemAction.GetDisplayName()))
                ;
        }
    }
}
