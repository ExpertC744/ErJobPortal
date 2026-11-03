
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class EditTPORegistrationM
    {
        // =====================================================
        // BASIC DETAILS
        // =====================================================

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(150)]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "College Email ID is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid college email address.")]
        [StringLength(200)]
        public string CollegeMailID { get; set; } = "";

        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression(
            @"^[0-9]{10}$",
            ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string MobileNo { get; set; } = "";


        // =====================================================
        // COLLEGE DETAILS
        // =====================================================

        [Required(ErrorMessage = "Please select a college.")]
        [RegularExpression(
            @"^[1-9][0-9]*$",
            ErrorMessage = "Please select a valid college.")]
        public string CollegeName { get; set; } = "";

        [StringLength(100)]
        public string? CollegeCode { get; set; }

        [Required(ErrorMessage = "College Address is required.")]
        [StringLength(500)]
        public string CollegeAddress { get; set; } = "";

        [Url(ErrorMessage = "Enter a valid website URL.")]
        [StringLength(500)]
        public string? OrganizationWebsiteURL { get; set; }


        // =====================================================
        // EMPLOYEE DETAILS
        // =====================================================

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(150)]
        public string Designation { get; set; } = "";

        [Required(ErrorMessage = "Please select a department.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a valid department.")]
        public int? nDepartment { get; set; }

        // Existing database column, if used elsewhere.
        public string? DepartmentName { get; set; }

        [Required(ErrorMessage = "College Employee ID is required.")]
        [StringLength(100)]
        public string CollegeEmployeeID { get; set; } = "";


        // =====================================================
        // OPTIONAL NEW FILE UPLOADS
        // =====================================================

        public IFormFile? SupportingDocument1File { get; set; }

        public IFormFile? SupportingDocument2File { get; set; }

        public IFormFile? ProfilePhotoFile { get; set; }


        // =====================================================
        // EXISTING SAVED FILE NAMES
        // Used for displaying existing uploads.
        // Never trust these values from a submitted form.
        // =====================================================

        public string? SupportingDocument1 { get; set; }

        public string? SupportingDocument2 { get; set; }

        public string? ProfilePhoto { get; set; }
    }
}
