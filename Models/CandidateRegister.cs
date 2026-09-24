using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class CandidateRegister
    {
        // =====================================================
        // FIRST NAME
        // =====================================================

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(
            100,
            ErrorMessage = "First name cannot exceed 100 characters."
        )]
        [RegularExpression(
            @"^[a-zA-Z\s.]+$",
            ErrorMessage = "First name can contain only letters, spaces and dots."
        )]
        public string? sFName { get; set; }


        // =====================================================
        // LAST NAME
        // =====================================================

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(
            100,
            ErrorMessage = "Last name cannot exceed 100 characters."
        )]
        [RegularExpression(
            @"^[a-zA-Z\s.]+$",
            ErrorMessage = "Last name can contain only letters, spaces and dots."
        )]
        public string? sLName { get; set; }


        // =====================================================
        // MOBILE
        // 6 TO 20 DIGITS
        // =====================================================

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(
            @"^[0-9]{6,20}$",
            ErrorMessage = "Mobile number must contain 6 to 20 digits."
        )]
        public string? sMobile { get; set; }


        // =====================================================
        // EMAIL
        // @ AND . COMPULSORY
        // =====================================================

        [Required(ErrorMessage = "Email address is required.")]
        [RegularExpression(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessage = "Email must contain @ and ."
        )]
        [StringLength(
            150,
            ErrorMessage = "Email cannot exceed 150 characters."
        )]
        public string? sEmail { get; set; }


        // =====================================================
        // DATE OF BIRTH
        // =====================================================

        [DataType(DataType.Date)]
        [CustomValidation(
            typeof(CandidateRegister),
            nameof(ValidateDOB)
        )]
        public DateTime? DOB { get; set; }


        // =====================================================
        // GENDER
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a gender."
        )]
        public int nGender { get; set; }


        // =====================================================
        // PROFILE IMAGE
        // =====================================================

        public string? sProfileImage { get; set; }

        [DataType(DataType.Upload)]
        [AllowedExtensions(
            new[] { ".jpg", ".jpeg", ".png", ".webp" },
            ErrorMessage = "Only JPG, JPEG, PNG and WEBP images are allowed."
        )]
        [MaxFileSize(
            5 * 1024 * 1024,
            ErrorMessage = "Profile image cannot exceed 5 MB."
        )]
        public IFormFile? ProfileImageFile { get; set; }


        // =====================================================
        // COLLEGE CODE
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a college code."
        )]
        public int nCollegeCode { get; set; }


        // =====================================================
        // COLLEGE NAME / ID
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a college."
        )]
        public int sCollegeName { get; set; }


        // =====================================================
        // DEPARTMENT
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a department."
        )]
        public int nDepartment { get; set; }


        // =====================================================
        // BRANCH
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a branch."
        )]
        public int nBranch { get; set; }


        // =====================================================
        // BROAD GROUP
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a broad group."
        )]
        public int nBroadGroup { get; set; }


        // =====================================================
        // PASSOUT YEAR
        // =====================================================

        [Range(
            1900,
            2100,
            ErrorMessage = "Please enter a valid passout year."
        )]
        public int? nPassoutYear { get; set; }


        // =====================================================
        // PASSWORD
        // =====================================================

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


        // =====================================================
        // OTP
        // =====================================================

        [Required(ErrorMessage = "OTP is required.")]
        [RegularExpression(
            @"^[0-9]{6}$",
            ErrorMessage = "OTP must be exactly 6 digits."
        )]
        public string? sOTP { get; set; }


        // =====================================================
        // CONFIRM PASSWORD
        // =====================================================

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare(
            "sPassword",
            ErrorMessage = "Password and confirm password do not match."
        )]
        public string? sConfirmPassword { get; set; }


        // =====================================================
        // REGISTRATION TYPE
        // =====================================================

        [StringLength(
            50,
            ErrorMessage = "Registration type cannot exceed 50 characters."
        )]
        public string? RegistrationType { get; set; }


        // =====================================================
        // DOB VALIDATION
        // =====================================================

        public static ValidationResult? ValidateDOB(
            DateTime? dob,
            ValidationContext context)
        {
            if (!dob.HasValue)
                return ValidationResult.Success;

            if (dob.Value.Date > DateTime.Today)
            {
                return new ValidationResult(
                    "Date of birth cannot be in the future."
                );
            }

            return ValidationResult.Success;
        }
    }


    // =========================================================
    // FILE EXTENSION VALIDATION
    // =========================================================

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is IFormFile file)
            {
                string extension =
                    Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult(
                        ErrorMessage ??
                        "Invalid file type."
                    );
                }
            }

            return ValidationResult.Success;
        }
    }


    // =========================================================
    // FILE SIZE VALIDATION
    // =========================================================

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxBytes;

        public MaxFileSizeAttribute(long maxBytes)
        {
            _maxBytes = maxBytes;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is IFormFile file)
            {
                if (file.Length > _maxBytes)
                {
                    return new ValidationResult(
                        ErrorMessage ??
                        "File size is too large."
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
}