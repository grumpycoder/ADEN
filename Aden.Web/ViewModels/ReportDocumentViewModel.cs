using Aden.Core.Models;
using Heroic.AutoMapper;

namespace Aden.Web.ViewModels
{
    public class ReportDocumentViewModel : IMapFrom<ReportDocument>
    {
        public int Id { get; set; }
        public byte[] File { get; set; }
        public string Filename { get; set; }


    }
}