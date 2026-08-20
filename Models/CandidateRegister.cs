namespace ErJobPortal.Models
{
    public class CandidateRegister
    {
        public string? sFName { get; set; }

        public string? sLName { get; set; }

        public string? sMobile { get; set; }

        public string? sEmail { get; set; }

        public DateTime? DOB { get; set; }

        public int nGender { get; set; }

        public string? sProfileImage { get; set; }

        public int nCollegeCode { get; set; }

        public int sCollegeName { get; set; }

        public string? sPassword { get; set; }


        // Additional Registration Fields

        public string? nBranch { get; set; }

        public int? nPassoutYear { get; set; }

        public string? sOTP { get; set; }

        public string? sConfirmPassword { get; set; }


        // Registration Type

        public string? RegistrationType { get; set; }


    }
}