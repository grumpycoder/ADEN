using Aden.Core.Dtos;
using Aden.Core.Helpers;
using Aden.Core.Models;
using AutoMapper;
using System.Text;

namespace Aden.Core.Profiles
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<ReportDocument, ReportDocumentDto>()
                .ForMember(d => d.Content, opt => opt.MapFrom(s => Encoding.UTF8.GetString(s.FileData).ToString()));

            CreateMap<Report, ReportDto>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.Submission.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.Submission.FileSpecification.FileNumber))
                .ForMember(d => d.Documents, opt => opt.MapFrom(s => s.Documents))
                .ForMember(d => d.ReportStateId, opt => opt.MapFrom(s => s.ReportState))
                .ForMember(d => d.ReportState, opt => opt.MapFrom(s => s.ReportState.GetDisplayName()))
                .ForMember(d => d.DataYear, opt => opt.MapFrom(s => s.DataYear))
                .ForAllOtherMembers(d => d.Ignore());

            CreateMap<WorkItem, ReportUploadDto>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileNumber));
        }
    }
}
