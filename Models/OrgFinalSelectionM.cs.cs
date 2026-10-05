namespace ErJobPortal.Models
{
    public class OrgFinalSelectionM
    {
        public int SrNo { get; set; }

        public int FinalSelectionID { get; set; }

        public int OrganizationID { get; set; }

        public int CandidateID { get; set; }

        public int PostID { get; set; }

        public DateTime? PostDate { get; set; }

        public string PositionName { get; set; }

        public string TraineeName { get; set; }

        public string EmailID { get; set; }

        public string TraineeSkills { get; set; }

        public int ResumeCandidateID { get; set; }
    }
}
