namespace ErJobPortal.Models
{
    public class EligibleTraineeM
    {
        public int CandidateID { get; set; }

        public string Name { get; set; }
        public string EmailID { get; set; }

        public int? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int PostID { get; set; }

        public int? GenderRequired { get; set; }

        public int? InternshipType { get; set; }

        public int? MinQualification { get; set; }

        public int? MaxQualification { get; set; }

        public string MedicalSkill { get; set; }
        public string TechnicalSkill { get; set; }
        public string NonTechnicalSkill { get; set; }

        public string MatchedSkillType { get; set; }

        public int? CApply { get; set; }

        public string Status { get; set; }

        public string FinalStatus { get; set; }

        public string Comment { get; set; }

        public string MedicalSkillNames { get; set; }

        public string TechnicalSkillNames { get; set; }

        public string NonTechnicalSkillNames { get; set; }
    }
}
