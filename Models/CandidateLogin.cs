
using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class CandidateLogin
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? sEmail { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string? sPassword { get; set; }

        public bool RememberMe { get; set; }
    }
}
