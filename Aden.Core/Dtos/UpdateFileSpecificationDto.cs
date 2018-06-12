using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace Aden.Core.Dtos
{
    public class UpdateFileSpecificationDto : IMapFrom<FileSpecification>, IHaveCustomMappings
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "File Number")]
        public string FileNumber { get; set; }
        [Display(Name = "File Name"), Required]
        public string FileName { get; set; }

        public string Section { get; set; }
        [Display(Name = "Data Groups")]
        public string DataGroups { get; set; }
        public string Application { get; set; }
        public string Collection { get; set; }
        public string DataSource { get; set; }

        [Display(Name = "Generation User Group")]
        public string GenerationUserGroup { get; set; }
        [Display(Name = "Approval User Group")]
        public string ApprovalUserGroup { get; set; }
        [Display(Name = "Submission User Group")]
        public string SubmissionUserGroup { get; set; }

        [Display(Name = "Filename Format")]
        public string FileNameFormat { get; set; }

        [Display(Name = "Report Action")]
        public string ReportAction { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FileSpecification, UpdateFileSpecificationDto>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.FileNumber))
                .ForMember(d => d.Section, opt => opt.MapFrom(s => s.Section))
                .ForMember(d => d.Application, opt => opt.MapFrom(s => s.Application))
                .ForMember(d => d.DataGroups, opt => opt.MapFrom(s => s.DataGroups))
                .ForMember(d => d.Collection, opt => opt.MapFrom(s => s.Collection))
                .ForMember(d => d.DataSource, opt => opt.MapFrom(s => s.DataSource))
                .ForMember(d => d.GenerationUserGroup, opt => opt.MapFrom(s => s.GenerationUserGroup))
                .ForMember(d => d.ApprovalUserGroup, opt => opt.MapFrom(s => s.ApprovalUserGroup))
                .ForMember(d => d.SubmissionUserGroup, opt => opt.MapFrom(s => s.SubmissionUserGroup))
                .ForMember(d => d.FileNameFormat, opt => opt.MapFrom(s => s.FileNameFormat))
                .ForMember(d => d.ReportAction, opt => opt.MapFrom(s => s.ReportAction)).ReverseMap()
                .ForAllOtherMembers(d => d.Ignore());
        }
    }
}
