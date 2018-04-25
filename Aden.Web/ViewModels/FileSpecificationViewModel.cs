using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace Aden.Web.ViewModels
{
    public class FileSpecificationViewModel : IMapFrom<FileSpecification>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }

        public bool? IsRetired { get; set; }
        public string FileNameFormat { get; set; }
        public string ReportAction { get; set; }
        public int? DataYear { get; set; }
        public string DisplayDataYear
        {
            get { return string.Format("{0}-{1}", DataYear - 1, DataYear); }
        }

        public string Department { get; set; }
        public string GenerationUserGroup { get; set; }
        public string ApprovalUserGroup { get; set; }
        public string SubmissionUserGroup { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FileSpecification, FileSpecificationViewModel>()
                //.ForMember(d => d.MostRecentReportId, opt => opt.MapFrom(s => s.Reports.OrderByDescending(r => r.Id).FirstOrDefault().Id))
                //.ForMember(d => d.ReportStateId, opt => opt.MapFrom(s => s.ReportState))
                //.ForMember(d => d.ReportState, opt => opt.MapFrom(s => s.ReportState.GetDisplayName()))
                ;

        }
    }
}