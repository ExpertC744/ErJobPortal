using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class TPOResetPassword
    {
        [Required(ErrorMessage = "Please enter your registered college email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string CollegeMailID { get; set; } = "";

        [Required(ErrorMessage = "Please enter your new password.")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters."
        )]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Please confirm your new password.")]
        [Compare(
            "NewPassword",
            ErrorMessage = "New password and confirm password do not match."
        )]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = "";
    }
}