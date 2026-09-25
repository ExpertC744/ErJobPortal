using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class SAFeedbackM
    {
        public int nID { get; set; }

        public string? Que1 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 1.")]
        [StringLength(1000, ErrorMessage = "Answer cannot exceed 1000 characters.")]
        public string? Ans1 { get; set; }


        public string? Que2 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 2.")]
        [StringLength(1000, ErrorMessage = "Answer cannot exceed 1000 characters.")]
        public string? Ans2 { get; set; }


        public string? Que3 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 3.")]
        [StringLength(1000, ErrorMessage = "Answer cannot exceed 1000 characters.")]
        public string? Ans3 { get; set; }


        public string? Que4 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 4.")]
        [StringLength(1000, ErrorMessage = "Answer cannot exceed 1000 characters.")]
        public string? Ans4 { get; set; }


        public string? Que5 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 5.")]
        [StringLength(1000, ErrorMessage = "Answer cannot exceed 1000 characters.")]
        public string? Ans5 { get; set; }


        public bool? nBit { get; set; }

        public bool? nSABit { get; set; }

        public int nSAID { get; set; }

        public DateTime? dRegDate { get; set; }

        public DateTime? dModDate { get; set; }
    }
}