using System.ComponentModel.DataAnnotations;
using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace Aden.Web.ViewModels
{
    public class ReportUploadViewModel : IMapFrom<WorkItem>, IHaveCustomMappings
    {
        public ReportUploadViewModel()
        {
            //Files = new List<HttpPostedFileBase>();
        }

        public int Id { get; set; }
        public int ReportId { get; set; }
        public int DataYear { get; set; }

        [Display(Name = "Data Year")]
        public string DisplayDataYear
        {
            get { return string.Format("{0}-{1}", DataYear - 1, DataYear); }
        }

        public WorkItemAction WorkItemActionId { get; set; }

        public string FileName { get; set; }
        public string FileNumber { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<WorkItem, ReportUploadViewModel>()
                .ForMember(d => d.DataYear, opt => opt.MapFrom(s => s.Report.Submission.DataYear))
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileNumber));
        }
    }
}