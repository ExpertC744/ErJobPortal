namespace ErJobPortal.Models
{
    public class TPOLoginResult
    {
        public int TPOID { get; set; }

        public string FullName { get; set; }

        public string CollegeMailID { get; set; }

        public string MobileNo { get; set; }

        public string PasswordHash { get; set; }

        public string CollegeName { get; set; }

        public string CollegeCode { get; set; }

        public string Designation { get; set; }

        public string DepartmentName { get; set; }

        public bool OTPVerified { get; set; }

        public string Status { get; set; }

        public bool IsApproved { get; set; }

        public bool IsActive { get; set; }
    }
}