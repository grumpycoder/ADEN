using Aden.Core.Helpers;
using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace Aden.Core.Dtos
{
    public class ReassignmentDto : IMapFrom<WorkItem>, IHaveCustomMappings
    {
        public int WorkItemId { get; set; }
        public string AssignedUser { get; set; }
        public string WorkItemAction { get; set; }
        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<WorkItem, ReassignmentDto>()
                .ForMember(d => d.WorkItemAction, opt => opt.MapFrom(s => s.WorkItemAction.GetDisplayName()));
        }
    }
}
