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

        public string Section { get; set; }
        public string DataGroups { get; set; }
        public string Application { get; set; }
        public string Collection { get; set; }
        public string DataSource { get; set; }


        public string GenerationUserGroup { get; set; }
        public string ApprovalUserGroup { get; set; }
        public string SubmissionUserGroup { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FileSpecification, FileSpecificationViewModel>()
                //.ForMember(d => d.MostRecentReportId, opt => opt.MapFrom(s => s.Reports.OrderByDescending(r => r.Id).FirstOrDefault().Id))
                //.ForMember(d => d.SubmissionStateId, opt => opt.MapFrom(s => s.SubmissionState))
                //.ForMember(d => d.SubmissionState, opt => opt.MapFrom(s => s.SubmissionState.GetDisplayName()))
                ;

        }
    }
}