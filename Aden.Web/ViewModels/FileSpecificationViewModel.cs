using System;
using ADEN.Web.Helpers;
using ADEN.Web.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace ADEN.Web.ViewModels
{
    public class FileSpecificationViewModel : IMapFrom<FileSpecification>, IHaveCustomMappings
    {
        public string FileName { get; set; }
        public string FileNumber { get; set; }
        public int DataYear { get; set; }
        public string State { get; set; }
        public DateTime DueDate { get; set; }
        public bool? IsSEA { get; set; }
        public bool? IsLEA { get; set; }
        public bool? IsSCH { get; set; }



        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FileSpecification, FileSpecificationViewModel>()
                .ForMember(d => d.State, opt => opt.MapFrom(s => s.ReportState.GetDisplayName()));

        }
    }
}