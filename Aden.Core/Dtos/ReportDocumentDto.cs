using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;
using System.Text;

namespace Aden.Core.Dtos
{
    public class ReportDocumentDto : IMapFrom<ReportDocument>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Filename { get; set; }

        public string Content { get; set; }

        public string Html => ConvertToHtml();

        private string ConvertToHtml()
        {
            var sb = new StringBuilder();
            sb.Append("<table><thead></thead>");

            sb.Append("</table>");
            return Content;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ReportDocument, ReportDocumentDto>()
                .ForMember(d => d.Content, opt => opt.MapFrom(s => Encoding.UTF8.GetString(s.FileData).ToString()));
        }
    }
}
