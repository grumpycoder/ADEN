using System.Collections.Generic;

namespace ADEN.Web.Models
{
    public class FileSpecification
    {
        public int Id { get; set; }
        public string FileNumber { get; set; }
        public string FileName { get; set; }
        public bool? IsRetired { get; set; }
        public string FileNameFormat { get; set; }
        public string ReportAction { get; set; }

        public string Department { get; set; }
        public string GenerationUserGroup { get; set; }
        public string ApprovalUserGroup { get; set; }
        public string SubmissionUserGroup { get; set; }

        public List<Submission> Submissions { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", FileNumber, FileName);
        }

        public void Retire()
        {
            IsRetired = true;
        }

        public void Activate()
        {
            IsRetired = false;
        }
    }
}
