using System.ComponentModel.DataAnnotations;

namespace JobPortalTrainee.Models
{
    public class OrgProfile
    {
        public int nID { get; set; }

        public int nOrgID { get; set; }


        [Required(ErrorMessage = "Organization name is required.")]
        [StringLength(
            150,
            ErrorMessage = "Organization name cannot exceed 150 characters."
        )]
        public string sOrganizationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(
        150,
        ErrorMessage = "Designation cannot exceed 150 characters."
    )]
        public string sDesignation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(
            @"^[0-9]{6,20}$",
            ErrorMessage = "Mobile number must contain 6 to 20 digits."
        )]
        public string sMobile { get; set; } = string.Empty;

        [Required(ErrorMessage = "Organization email is required.")]
        [RegularExpression(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessage = "Please enter a valid email address containing @ and ."
        )]
        [StringLength(
            150,
            ErrorMessage = "Email cannot exceed 150 characters."
        )]
        public string sOrganizationEmail { get; set; } = string.Empty;


        public DateTime? dDateOfBirth { get; set; }

        public string? sCompanyLogo { get; set; }

        [Required(ErrorMessage = "Company address is required.")]
        [StringLength(
        500,
        ErrorMessage = "Company address cannot exceed 500 characters."
    )]
        public string sCompanyAddress { get; set; } = string.Empty;

        [Range(
       1800,
       2100,
       ErrorMessage = "Please enter a valid establishment year."
   )]
        public int nEstablishmentYear { get; set; }

        [RegularExpression(
        @"^[0-9A-Z]{15}$",
        ErrorMessage = "GST number must contain exactly 15 uppercase letters/numbers."
    )]
        public string? sGSTNo { get; set; }


        [RegularExpression(
            @"^[A-Z0-9]{21}$",
            ErrorMessage = "CIN number must contain exactly 21 uppercase letters/numbers."
        )]
        public string? sCINNo { get; set; }

        [Required(ErrorMessage = "Employee strength is required.")]
        [StringLength(
         50,
         ErrorMessage = "Employee strength cannot exceed 50 characters."
     )]
        public string nEmployeeStrength { get; set; } = string.Empty;

        public DateTime dCreatedDate { get; set; }

        public DateTime? dModifiedDate { get; set; }

        public bool nBit { get; set; }

        public bool? nSABit { get; set; }
    }
}