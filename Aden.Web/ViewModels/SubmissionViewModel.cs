using System;
using System.Collections.Generic;
using System.Linq;
using ADEN.Web.Helpers;
using ADEN.Web.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace Aden.Web.ViewModels
{
    public class SubmissionViewModel : IMapFrom<Submission>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public int? DataYear { get; set; }

        public bool IsSEA { get; set; }
        public bool IsLEA { get; set; }
        public bool IsSCH { get; set; }

        public string FileNumber { get; set; }
        public string FileName { get; set; }

        public string ReportState { get; set; }
        public ReportState ReportStateId { get; set; }

        public int? MostRecentReportId { get; set; }

        public virtual List<Report> Reports { get; set; }

        public byte[] SpecificationDocument { get; set; }

        public int FileSpecificationId { get; set; }
        public virtual FileSpecification FileSpecification { get; set; }

        public bool CanStartReport
        {
            get { return !string.IsNullOrEmpty(FileSpecification.ReportAction); }
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Submission, SubmissionViewModel>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.FileSpecification.FileNumber))
                .ForMember(d => d.MostRecentReportId, opt => opt.MapFrom(s => s.Reports.OrderByDescending(r => r.Id).FirstOrDefault().Id))
                .ForMember(d => d.ReportStateId, opt => opt.MapFrom(s => s.ReportState))
                .ForMember(d => d.ReportState, opt => opt.MapFrom(s => s.ReportState.GetDisplayName()))
                ;
        }


    }
}