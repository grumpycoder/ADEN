namespace ADEN.Web.Models
{
    public class ReportDocument
    {
        public int Id { get; set; }
        public ReportLevel ReportLevel { get; set; }
        public byte[] File { get; set; }
        public string Filename { get; set; }
        public int Version { get; set; }
        public int ReportId { get; set; }
        public Report Report { get; set; }


        private ReportDocument()
        {

        }

        public static ReportDocument Create(string filename, int version, ReportLevel reportLevel, byte[] file)
        {
            var doc = new ReportDocument()
            {
                File = file,
                ReportLevel = reportLevel,
                Version = version,
                Filename = filename
            };
            return doc;
        }

    }
}