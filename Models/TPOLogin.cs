using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class TPOLogin
    {
        // =========================================================
        // COLLEGE EMAIL
        // =========================================================

        [Required(ErrorMessage = "College Mail ID is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid college email address.")]
        [StringLength(200)]
        public string CollegeMailID { get; set; }


        // =========================================================
        // PASSWORD
        // =========================================================

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters."
        )]
        public string Password { get; set; }
    }
}