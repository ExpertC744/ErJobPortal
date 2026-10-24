namespace ErJobPortal.Models
{
    public class CandidateUser
    {
        public int nID { get; set; }

        public string? sFName { get; set; }

        public string? sLName { get; set; }

        public string? sMobile { get; set; }

        public string? sEmail { get; set; }

        public DateTime? DOB { get; set; }

        public int nGender { get; set; }

        public string? sProfileImage { get; set; }

        public int nCollegeCode { get; set; }

        // FIX: College name must be a string.
        public string? sCollegeName { get; set; }

        // Stored password hash, if required by the login flow.
        public string? sPassword { get; set; }

        // Super Admin Status
        public bool nSABit { get; set; }
    }
}