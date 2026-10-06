namespace ErJobPortal.Models
{
    public class SAFinalSelectionViewModel
    {
        public int nID { get; set; }

        public int OrgID { get; set; }

        public int CandidateID { get; set; }

        public int PostID { get; set; }

        public DateTime? PostDate { get; set; }

        public string PositionName { get; set; } = string.Empty;

        public string CandidateName { get; set; } = string.Empty;

        public string EmailID { get; set; } = string.Empty;

        public string Skills { get; set; } = string.Empty;
    }
}