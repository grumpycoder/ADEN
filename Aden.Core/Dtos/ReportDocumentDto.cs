using Aden.Core.Models;
using Heroic.AutoMapper;

namespace Aden.Core.Dtos
{
    public class ReportDocumentDto : IMapFrom<ReportDocument>
    {
        public int Id { get; set; }
        //public byte[] File { get; set; }
        public string Filename { get; set; }
    }
}
