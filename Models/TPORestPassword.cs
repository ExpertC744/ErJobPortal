using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class TPORestPassword
    {
        [Required(ErrorMessage = "College Mail ID is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid college email address.")]
        public string CollegeMailID { get; set; } = "";

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters."
        )]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Compare(
            "NewPassword",
            ErrorMessage = "New password and confirm password do not match."
        )]
        public string ConfirmPassword { get; set; } = "";
    }
}