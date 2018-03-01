using System;
using System.ComponentModel.DataAnnotations;
using ADEN.Web.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace ADEN.Web.ViewModels
{
    public class FileSpecificationEditViewModel : IMapFrom<FileSpecification>, IHaveCustomMappings
    {
        public int Id { get; set; }
        [Required]
        public string FileNumber { get; set; }
        public string FileName { get; set; }
        public string Version { get; set; }

        public bool IsSEA { get; set; }
        public bool IsLEA { get; set; }
        public bool IsSCH { get; set; }

        public string Department { get; set; }
        public string GenerationUserGroup { get; set; }
        public string ApprovalUserGroup { get; set; }
        public string SubmissionUserGroup { get; set; }

        public bool? IsRetired { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DataYear { get; set; }
        public string FileNameFormat { get; set; }
        public string ReportAction { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FileSpecification, FileSpecificationViewModel>()
                //.ForMember(d => d.MostRecentReportId, opt => opt.MapFrom(s => s.Reports.OrderByDescending(r => r.Id).FirstOrDefault().Id))
                //.ForMember(d => d.ReportStateId, opt => opt.MapFrom(s => s.ReportState))
                //.ForMember(d => d.ReportState, opt => opt.MapFrom(s => s.ReportState.GetDisplayName())).ReverseMap()
                ;

        }
    }
}