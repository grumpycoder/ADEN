using Aden.Core.Dtos;
using Aden.Core.Helpers;
using Aden.Core.Models;
using AutoMapper;

namespace Aden.Core.Profiles
{
    public class WorkItemProfile : Profile
    {
        public WorkItemProfile()
        {
            CreateMap<WorkItem, ReassignmentDto>()
                .ForMember(d => d.WorkItemAction, opt => opt.MapFrom(s => s.WorkItemAction.GetDisplayName()))
                .ForMember(d => d.AssignedUser, opt => opt.MapFrom(s => s.AssignedUser))
                .ForAllOtherMembers(d => d.Ignore());

            CreateMap<WorkItem, WorkItemDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.DataYear, opt => opt.MapFrom(s => s.Report.Submission.DataYear))
                .ForMember(d => d.CanCancel, opt => opt.MapFrom(s => s.CanCancel))
                .ForMember(d => d.DueDate, opt => opt.MapFrom(s => s.Report.Submission.DueDate))
                .ForMember(d => d.AssignedDate, opt => opt.MapFrom(s => s.AssignedDate))
                .ForMember(d => d.CompletedDate, opt => opt.MapFrom(s => s.CompletedDate))
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => $"{s.Report.Submission.FileSpecification.FileName} ({s.Report.Submission.FileSpecification.FileNumber})"))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileNumber))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.WorkItemActionId, opt => opt.MapFrom(s => s.WorkItemAction))
                .ForMember(d => d.ReportAction, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.ReportAction))
                .ForMember(d => d.Action, opt => opt.MapFrom(s => s.WorkItemAction.GetShortName()))
                .ForMember(d => d.ActionName, opt => opt.MapFrom(s => s.WorkItemAction.GetDisplayName()))
                .ForMember(d => d.ActionDescription, opt => opt.MapFrom(s => s.WorkItemAction.GetDescription()))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.WorkItemState.GetDisplayName()))
                .ForAllOtherMembers(d => d.Ignore())
                ;

            CreateMap<WorkItem, WorkItemHistoryDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.AssignedDate, opt => opt.MapFrom(s => s.AssignedDate))
                .ForMember(d => d.AssignedUser, opt => opt.MapFrom(s => s.AssignedUser))
                .ForMember(d => d.WorkItemState, opt => opt.MapFrom(s => s.WorkItemState))
                .ForMember(d => d.CompletedDate, opt => opt.MapFrom(s => s.CompletedDate))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.Action, opt => opt.MapFrom(s => s.WorkItemAction))
                .ForAllOtherMembers(d => d.Ignore())
                ;
        }
    }
}
