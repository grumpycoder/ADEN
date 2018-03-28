using System;
using System.Collections.Generic;
using Aden.Core.Models;
using Aden.Web.Helpers;
using AutoMapper;
using Heroic.AutoMapper;

namespace Aden.Web.ViewModels
{
    public class ReportViewModel : IMapFrom<Report>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }
        public int DataYear { get; set; }
        public ReportState ReportStateId { get; set; }
        public string ReportState { get; set; }
        public int FileSpecificationId { get; set; }
        public List<ReportDocumentViewModel> Documents { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Report, ReportViewModel>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.Submission.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.Submission.FileSpecification.FileNumber))
                .ForMember(d => d.Documents, opt => opt.MapFrom(s => s.Documents))
                .ForMember(d => d.ReportStateId, opt => opt.MapFrom(s => s.ReportState))
                .ForMember(d => d.ReportState, opt => opt.MapFrom(s => s.ReportState.GetDisplayName()));

        }
    }
}