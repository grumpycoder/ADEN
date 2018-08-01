using Aden.Core.Helpers;
using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Aden.Core.Dtos
{
    public class SubmissionDto : IMapFrom<Submission>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string Section { get; set; }
        public string DataGroups { get; set; }
        public string Application { get; set; }
        public string Collection { get; set; }
        public string DataSource { get; set; }

        public int? DataYear { get; set; }

        public string DisplayDataYear => string.Format("{0}-{1}", DataYear - 1, DataYear);

        public bool IsSEA { get; set; }
        public bool IsLEA { get; set; }
        public bool IsSCH { get; set; }

        public string FileNumber { get; set; }
        public string FileName { get; set; }

        public string SubmissionState { get; set; }
        public string SubmissionStateKey { get; set; }
        public SubmissionState SubmissionStateId { get; set; }

        public int? MostRecentReportId { get; set; }

        public virtual List<Report> Reports { get; set; }

        public byte[] SpecificationDocument { get; set; }

        public int FileSpecificationId { get; set; }
        public virtual FileSpecification FileSpecification { get; set; }

        public bool CanStartReport => !string.IsNullOrEmpty(FileSpecification.ReportAction);

        public bool HasAdmin
        {
            get
            {
                var claim = (HttpContext.Current.User as ClaimsPrincipal).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value.Contains("MarkAdenAppAdministrators"));
                return claim != null;
            }
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Submission, SubmissionDto>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.FileSpecification.FileNumber))
                .ForMember(d => d.Section, opt => opt.MapFrom(s => s.FileSpecification.Section))
                .ForMember(d => d.DataGroups, opt => opt.MapFrom(s => s.FileSpecification.DataGroups))
                .ForMember(d => d.Application, opt => opt.MapFrom(s => s.FileSpecification.Application))
                .ForMember(d => d.Collection, opt => opt.MapFrom(s => s.FileSpecification.Collection))
                .ForMember(d => d.DataSource, opt => opt.MapFrom(s => s.FileSpecification.DataSource))
                .ForMember(d => d.MostRecentReportId,
                    opt => opt.MapFrom(s => s.Reports.OrderByDescending(r => r.Id).FirstOrDefault().Id))
                .ForMember(d => d.SubmissionStateId, opt => opt.MapFrom(s => s.SubmissionState))
                .ForMember(d => d.SubmissionState, opt => opt.MapFrom(s => s.SubmissionState.GetDisplayName()))
                .ForMember(d => d.SubmissionStateKey, opt => opt.MapFrom(s => s.SubmissionState.GetShortName()))
                ;
            ;
        }
    }
}
