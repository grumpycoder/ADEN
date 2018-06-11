using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;

namespace Aden.Core.Dtos
{
    public class ReportUploadDto : IMapFrom<WorkItem>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileNumber { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<WorkItem, ReportUploadDto>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.Report.Submission.FileSpecification.FileNumber));
        }
    }
}
