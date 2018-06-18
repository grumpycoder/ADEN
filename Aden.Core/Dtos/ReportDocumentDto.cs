using System.Collections.Generic;
using Aden.Core.Helpers;
using Aden.Core.Models;
using AutoMapper;
using Heroic.AutoMapper;
using Nortal.Utilities.Csv;
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

            sb.Append("<table class='table table-condensed table-responsive table-striped table-bordered'><thead></thead><tbody>");

            using (var parser = new CsvParser(Content))
            {
                var headerRow = parser.ReadNextRow();
                var firstRow = parser.ReadNextRow();

                sb.Append(CreateTableRow(headerRow, firstRow.Length));
                sb.Append(CreateTableRow(firstRow));

                var rows = parser.ReadToEnd();
                foreach (var row in rows)
                {
                    sb.Append(CreateTableRow(row));
                }
            }


            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        private string CreateTableRow(IReadOnlyCollection<string> row, int? numberCols = null)
        {
            var extraCols = numberCols - row.Count;

            var sb = new StringBuilder();
            sb.Append("<tr>");
            foreach (var col in row)
            {
                sb.AppendFormat($"<td>{col}</td>");
            }

            sb.Append("<td></td>".Repeat(extraCols ?? 0));
            sb.Append("</tr>");
            return sb.ToString();
        }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ReportDocument, ReportDocumentDto>()
                .ForMember(d => d.Content, opt => opt.MapFrom(s => Encoding.UTF8.GetString(s.FileData).ToString()));
        }
    }
}
