using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class TPOForgotPassword
    {
        [Required(
            ErrorMessage = "Please enter your registered college email address.")]
        [EmailAddress(
            ErrorMessage = "Please enter a valid email address.")]
        public string CollegeMailID { get; set; } = "";
    }
}