using System;

namespace ErJobPortal.Models
{
    public class TPOTraineeInfoM
    {
        public int nID { get; set; }

        // =====================================================
        // PREFIX
        // =====================================================

        public int sPrefix { get; set; }

        public string PrefixName
        {
            get
            {
                return sPrefix switch
                {
                    1 => "Mr.",
                    2 => "Ms.",
                    3 => "Mrs.",
                    4 => "Dr.",
                    5 => "Prof.",
                    _ => ""
                };
            }
        }

        // =====================================================
        // NAME
        // =====================================================

        public string? sFName { get; set; }

        public string? sLName { get; set; }

        public string FullName
        {
            get
            {
                return $"{sFName} {sLName}".Trim();
            }
        }

        // =====================================================
        // CONTACT
        // =====================================================

        public string? sMobile { get; set; }

        public string? sEmail { get; set; }

        // =====================================================
        // PERSONAL INFORMATION
        // =====================================================

        public DateTime? DOB { get; set; }

        public int nGender { get; set; }

        public string? GenderName { get; set; }

        // =====================================================
        // PROFILE IMAGE
        // =====================================================

        public string? sProfileImage { get; set; }

        // =====================================================
        // COLLEGE
        // =====================================================

        public int nCollegeName { get; set; }

        public int nCollegeCode { get; set; }

        // =====================================================
        // DEPARTMENT / BRANCH
        // =====================================================

        public int? nDepartment { get; set; }

        public int? nBranch { get; set; }

        public string? BranchName { get; set; }

        // =====================================================
        // EDUCATION
        // =====================================================

        public int? nCurrentYear { get; set; }

        public int? nAdmissionYear { get; set; }

        public int? nPassoutYear { get; set; }

        // =====================================================
        // REGISTRATION
        // =====================================================

        public DateTime? RegDate { get; set; }

        public bool nSABit { get; set; }

        // =====================================================
        // RESUME
        // =====================================================

        public string? SelectedResume { get; set; }

        // =====================================================
        // CANDIDATE CODE
        // =====================================================

        public string? CandidateCode { get; set; }
    }
}