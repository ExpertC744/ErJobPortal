using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class SAOrgFeedbackM
    {
        // =====================================================
        // ID
        // =====================================================

        public int nID { get; set; }


        // =====================================================
        // QUESTION 1
        // =====================================================

        public string? Que1 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 1.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 1 cannot exceed 1000 characters."
        )]
        public string? Ans1 { get; set; }


        // =====================================================
        // QUESTION 2
        // =====================================================

        public string? Que2 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 2.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 2 cannot exceed 1000 characters."
        )]
        public string? Ans2 { get; set; }


        // =====================================================
        // QUESTION 3
        // =====================================================

        public string? Que3 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 3.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 3 cannot exceed 1000 characters."
        )]
        public string? Ans3 { get; set; }


        // =====================================================
        // QUESTION 4
        // =====================================================

        public string? Que4 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 4.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 4 cannot exceed 1000 characters."
        )]
        public string? Ans4 { get; set; }


        // =====================================================
        // QUESTION 5
        // =====================================================

        public string? Que5 { get; set; }

        [Required(ErrorMessage = "Please enter answer for Question 5.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 5 cannot exceed 1000 characters."
        )]
        public string? Ans5 { get; set; }


        // =====================================================
        // STATUS
        // =====================================================

        public bool nBit { get; set; }

        public bool nSABit { get; set; }


        // =====================================================
        // SUPER ADMIN ID
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Invalid Super Admin ID."
        )]
        public int nSAID { get; set; }


        // =====================================================
        // DATES
        // =====================================================

        public DateTime? dRegDate { get; set; }

        public DateTime? dModDate { get; set; }
    }
}