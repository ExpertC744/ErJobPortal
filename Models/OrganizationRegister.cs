using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class OrganizationRegister
    {
        [Required(ErrorMessage = "Organization name is required.")]
        [StringLength(150, ErrorMessage = "Organization name cannot exceed 150 characters.")]
        public string? sOrgName { get; set; }


        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(200, ErrorMessage = "Website URL cannot exceed 200 characters.")]
        public string? sOrgUrl { get; set; }


        [Required(ErrorMessage = "Contact person name is required.")]
        [StringLength(150, ErrorMessage = "Contact person name cannot exceed 150 characters.")]
        [RegularExpression(
            @"^[a-zA-Z\s.]+$",
            ErrorMessage = "Contact person name can contain only letters, spaces and dots."
        )]
        public string? sName { get; set; }


        [StringLength(150, ErrorMessage = "Designation cannot exceed 150 characters.")]
        public string? sDesignation { get; set; }


        // 6 to 20 digits allowed
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(
            @"^[0-9]{6,20}$",
            ErrorMessage = "Mobile number must contain 6 to 20 digits."
        )]
        public string? sMobile { get; set; }


        // @ and . are compulsory
        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessage = "Email must contain @ and ."
        )]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string? sEmail { get; set; }


        [Required(ErrorMessage = "OTP is required.")]
        [RegularExpression(
            @"^[0-9]{6}$",
            ErrorMessage = "OTP must be exactly 6 digits."
        )]
        public string? sOTP { get; set; }


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a college code."
        )]
        public int nCollegeCode { get; set; }


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a college name."
        )]
        public int nCollegeName { get; set; }


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a department."
        )]
        public int nDepartment { get; set; }


        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a branch."
        )]
        public int nBranch { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(
            200,
            MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 200 characters."
        )]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character."
        )]
        public string? sPassword { get; set; }
    }
}