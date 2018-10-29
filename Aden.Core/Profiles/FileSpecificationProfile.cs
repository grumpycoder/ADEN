using Aden.Core.Dtos;
using Aden.Core.Models;
using AutoMapper;

namespace Aden.Core.Profiles
{
    public class FileSpecificationProfile : Profile
    {
        public FileSpecificationProfile()
        {
            CreateMap<FileSpecification, FileSpecificationDto>();

            CreateMap<FileSpecification, UpdateFileSpecificationDto>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.FileNumber))
                .ForMember(d => d.Section, opt => opt.MapFrom(s => s.Section))
                .ForMember(d => d.Application, opt => opt.MapFrom(s => s.Application))
                .ForMember(d => d.DataGroups, opt => opt.MapFrom(s => s.DataGroups))
                .ForMember(d => d.Collection, opt => opt.MapFrom(s => s.Collection))
                .ForMember(d => d.SupportGroup, opt => opt.MapFrom(s => s.SupportGroup))
                .ForMember(d => d.GenerationUserGroup, opt => opt.MapFrom(s => s.GenerationUserGroup))
                .ForMember(d => d.ApprovalUserGroup, opt => opt.MapFrom(s => s.ApprovalUserGroup))
                .ForMember(d => d.SubmissionUserGroup, opt => opt.MapFrom(s => s.SubmissionUserGroup))
                .ForMember(d => d.FileNameFormat, opt => opt.MapFrom(s => s.FileNameFormat))
                .ForMember(d => d.ReportAction, opt => opt.MapFrom(s => s.ReportAction)).ReverseMap()
                .ForAllOtherMembers(d => d.Ignore()); ;

        }
    }
}
