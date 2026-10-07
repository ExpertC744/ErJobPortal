using System;

namespace ErJobPortal.Models
{
    public class TPOTraineeInfoM
    {
        public int nID { get; set; }

        public string? sFName { get; set; }

        public string? sLName { get; set; }

        public string FullName
        {
            get
            {
                return $"{sFName} {sLName}".Trim();
            }
        }

        public string? sMobile { get; set; }

        public string? sEmail { get; set; }

        public DateTime? DOB { get; set; }

        public int nGender { get; set; }

        public string? sProfileImage { get; set; }

        public int nCollegeCode { get; set; }

        public int nCollegeName { get; set; }

        public int? nDepartment { get; set; }

        public int? nBranch { get; set; }

        public int? nCurrentYear { get; set; }

        public int? nAdmissionYear { get; set; }

        public int? nPassoutYear { get; set; }

        public DateTime? RegDate { get; set; }

        public bool nSABit { get; set; }
    }
}