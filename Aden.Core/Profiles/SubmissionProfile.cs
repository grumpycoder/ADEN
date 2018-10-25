using Aden.Core.Dtos;
using Aden.Core.Helpers;
using Aden.Core.Models;
using AutoMapper;
using System.Linq;


namespace Aden.Core.Profiles
{
    public class SubmissionProfile : Profile
    {
        public SubmissionProfile()
        {
            //CreateMap<Submission, SubmissionDto>()
            //    .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.FileSpecification.FileName))
            //    .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.FileSpecification.FileNumber))
            //    .ForMember(d => d.Section, opt => opt.MapFrom(s => s.FileSpecification.Section))
            //    .ForMember(d => d.DataGroups, opt => opt.MapFrom(s => s.FileSpecification.DataGroups))
            //    .ForMember(d => d.Application, opt => opt.MapFrom(s => s.FileSpecification.Application))
            //    .ForMember(d => d.Collection, opt => opt.MapFrom(s => s.FileSpecification.Collection))
            //    .ForMember(d => d.DataSource, opt => opt.MapFrom(s => s.FileSpecification.DataSource))
            //    .ForMember(d => d.MostRecentReportId,
            //        opt => opt.MapFrom(s => s.Reports.OrderByDescending(r => r.Id).FirstOrDefault().Id))
            //    .ForMember(d => d.SubmissionStateId, opt => opt.MapFrom(s => s.SubmissionState))
            //    .ForMember(d => d.SubmissionState, opt => opt.MapFrom(s => s.SubmissionState.GetDisplayName()))
            //    .ForMember(d => d.SubmissionStateKey, opt => opt.MapFrom(s => s.SubmissionState.GetShortName()))
            //    .ForMember(d => d.LastUpdated, opt => opt.MapFrom(s => s.LastUpdated))
            //    ;

            CreateMap<Submission, SubmissionViewDto>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.FileSpecification.FileNumber))
                .ForMember(d => d.LastUpdated, opt => opt.MapFrom(s => s.LastUpdated))
                .ForMember(d => d.DueDate, opt => opt.MapFrom(s => s.DueDate))
                .ForMember(d => d.NextDueDate, opt => opt.MapFrom(s => s.NextDueDate))
                .ForMember(d => d.DataYear, opt => opt.MapFrom(s => s.DataYear))
                .ForMember(d => d.Section, opt => opt.MapFrom(s => s.FileSpecification.Section))
                .ForMember(d => d.DataGroups, opt => opt.MapFrom(s => s.FileSpecification.DataGroups))
                .ForMember(d => d.Application, opt => opt.MapFrom(s => s.FileSpecification.Application))
                .ForMember(d => d.Collection, opt => opt.MapFrom(s => s.FileSpecification.Collection))
                .ForMember(d => d.DataSource, opt => opt.MapFrom(s => s.FileSpecification.DataSource))
                .ForMember(d => d.ReportAction, opt => opt.MapFrom(s => s.FileSpecification.ReportAction))
                .ForMember(d => d.GenerationUserGroup, opt => opt.MapFrom(s => s.FileSpecification.GenerationUserGroup))
                .ForMember(d => d.ApprovalUserGroup, opt => opt.MapFrom(s => s.FileSpecification.ApprovalUserGroup))
                .ForMember(d => d.SubmissionUserGroup, opt => opt.MapFrom(s => s.FileSpecification.SubmissionUserGroup))
                //.ForMember(d => d.MostRecentReportId,
                //    opt => opt.MapFrom(s => s.Reports.OrderByDescending(r => r.Id).FirstOrDefault().Id))
                //.ForMember(d => d.SubmissionStateId, opt => opt.MapFrom(s => s.SubmissionState))
                .ForMember(d => d.SubmissionState, opt => opt.MapFrom(s => s.SubmissionState))
                //.ForMember(d => d.SubmissionStateKey, opt => opt.MapFrom(s => s.SubmissionState.GetShortName()))
                ;
        }
    }
}
