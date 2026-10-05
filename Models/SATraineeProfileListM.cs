namespace ErJobPortal.Models
{
    public class SATraineeProfileListM
    {
        public int SrNo { get; set; }

        public int CandidateID { get; set; }

        public string? TraineeName { get; set; }

        public DateTime? ProfileCreatedDate { get; set; }

        public string? Photo { get; set; }

        public string? Location { get; set; }

        public string? Pincode { get; set; }

        public int? SSC_YEAR { get; set; }
        public int? SSC_DIVISION { get; set; }

        public int? HSC_DIPLOMA_YEAR { get; set; }
        public int? HSC_DIPLOMA_DIVISION { get; set; }

        public int? Graduation_Year { get; set; }
        public int? Graduation_Division { get; set; }
        public int? Graduation_Stream { get; set; }

        public int? PG_Year { get; set; }
        public int? PG_Division { get; set; }
        public int? PG_Stream { get; set; }

        public int? PhD_Year { get; set; }
        public int? PhD_Status { get; set; }
        public string? PhD_Topic { get; set; }

        public string? PreferredLocation { get; set; }

        public int? Internship_FellowshipType { get; set; }

        public string? Resume { get; set; }

        public bool ResumeUploaded { get; set; }
    }
}
