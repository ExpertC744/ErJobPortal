namespace ErJobPortal.Models
{
    public class TraineeProfileListM
    {
        public int SrNo { get; set; }

        public int CandidateID { get; set; }

        public string CandidateName { get; set; } = "";

        public string Photo { get; set; } = "";

        public string TechnicalSkill { get; set; } = "";

        public string NonTechnicalSkill { get; set; } = "";

        public string MedicalSkill { get; set; } = "";

        public string Country { get; set; } = "";

        public string State { get; set; } = "";

        public string City { get; set; } = "";
    }
}
