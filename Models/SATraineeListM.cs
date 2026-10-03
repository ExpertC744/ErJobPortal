namespace ErJobPortal.Models
{
    public class SATraineeListM
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

        public int sCollegeName { get; set; }

        public string? sPassword { get; set; }

        public DateTime? RegDate { get; set; }

        public DateTime? ModDate { get; set; }

        public bool nBit { get; set; }

        public bool nSABit { get; set; }

        public string? sOTP { get; set; }

        public int? nBranch { get; set; }

        public int? nPassoutYear { get; set; }

        public int? nDepartment { get; set; }
    }
}