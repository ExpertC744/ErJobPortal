
using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class EditOrganizationRegistrationM
    {
        // ============================================
        // ORGANIZATION NAME
        // ============================================

        [Required(ErrorMessage = "Organization name is required.")]
        [StringLength(150,
            ErrorMessage = "Organization name cannot exceed 150 characters.")]
        public string? sOrgName { get; set; }


        // ============================================
        // ORGANIZATION WEBSITE
        // ============================================

        [Required(ErrorMessage = "Organization website is required.")]
        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(200,
            ErrorMessage = "Website URL cannot exceed 200 characters.")]
        public string? sOrgUrl { get; set; }


        // ============================================
        // CONTACT PERSON NAME
        // ============================================

        [Required(ErrorMessage = "Contact person name is required.")]
        [StringLength(150,
            ErrorMessage = "Contact person name cannot exceed 150 characters.")]
        [RegularExpression(
            @"^[a-zA-Z\s.]+$",
            ErrorMessage = "Contact person name can contain only letters, spaces and dots.")]
        public string? sName { get; set; }


        // ============================================
        // DESIGNATION
        // ============================================

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(150,
            ErrorMessage = "Designation cannot exceed 150 characters.")]
        public string? sDesignation { get; set; }


        // ============================================
        // MOBILE NUMBER
        // ============================================

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(
            @"^[0-9]{10}$",
            ErrorMessage = "Mobile number must contain exactly 10 digits.")]
        public string? sMobile { get; set; }


        // ============================================
        // EMAIL ADDRESS
        // ============================================

        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(
            @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
            ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150,
            ErrorMessage = "Email cannot exceed 150 characters.")]
        public string? sEmail { get; set; }


        // ============================================
        // COLLEGE NAME
        // ============================================

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a college name.")]
        public int nCollegeName { get; set; }


        // ============================================
        // COLLEGE CODE
        // ============================================

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a college code.")]
        public int nCollegeCode { get; set; }


        // ============================================
        // DEPARTMENT
        // ============================================

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a department.")]
        public int nDepartment { get; set; }


        // ============================================
        // BRANCH
        // ============================================

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a branch.")]
        public int nBranch { get; set; }
    }
}
