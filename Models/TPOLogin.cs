using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class TPOLogin
    {
        // =========================================================
        // COLLEGE EMAIL
        // =========================================================

        [Required(ErrorMessage = "Please enter your College Mail ID.")]
        [EmailAddress(ErrorMessage = "Enter a valid college email address.")]
        [StringLength(200)]
        public string CollegeMailID { get; set; }


        // =========================================================
        // PASSWORD
        // =========================================================

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters."
        )]
        public string Password { get; set; }
    }
}