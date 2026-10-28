using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class OrganizationRegister
    {
        [Required(ErrorMessage = "Organization name is required.")]
        [StringLength(150, ErrorMessage = "Organization name cannot exceed 150 characters.")]
        public string? sOrgName { get; set; }

        [Required(ErrorMessage = "Organization website is required.")]
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

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(150, ErrorMessage = "Designation cannot exceed 150 characters.")]
        public string? sDesignation { get; set; }


        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(
@"^[0-9]{10}$",
ErrorMessage = "Mobile number must contain exactly 10 digits."
)]
        [Remote(
action: "CheckOrganizationMobile",
controller: "Account",
ErrorMessage = "This mobile number is already registered."
)]
        public string? sMobile { get; set; }


        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(
@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
ErrorMessage = "Please enter a valid email address, e.g., example@gmail.com."
)]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [Remote(
action: "CheckOrganizationEmail",
controller: "Account",
ErrorMessage = "This email address is already registered."
)]
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