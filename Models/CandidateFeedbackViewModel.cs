using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class CandidateFeedbackViewModel
    {
        // =====================================================
        // QUESTION ID
        // =====================================================

        public int nID { get; set; }


        // =====================================================
        // QUESTIONS
        // Questions come from database
        // =====================================================

        public string? Que1 { get; set; }

        public string? Que2 { get; set; }

        public string? Que3 { get; set; }

        public string? Que4 { get; set; }

        public string? Que5 { get; set; }


        // =====================================================
        // ANSWERS
        // =====================================================

        [Required(ErrorMessage = "Please answer Question 1.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 1 cannot exceed 1000 characters."
        )]
        public string? sQue1 { get; set; }


        [Required(ErrorMessage = "Please answer Question 2.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 2 cannot exceed 1000 characters."
        )]
        public string? sQue2 { get; set; }


        [Required(ErrorMessage = "Please answer Question 3.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 3 cannot exceed 1000 characters."
        )]
        public string? sQue3 { get; set; }


        [Required(ErrorMessage = "Please answer Question 4.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 4 cannot exceed 1000 characters."
        )]
        public string? sQue4 { get; set; }


        [Required(ErrorMessage = "Please answer Question 5.")]
        [StringLength(
            1000,
            ErrorMessage = "Answer 5 cannot exceed 1000 characters."
        )]
        public string? sQue5 { get; set; }


        // =====================================================
        // CANDIDATE
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Invalid candidate."
        )]
        public int? nCandidateID { get; set; }


        // =====================================================
        // ORGANIZATION
        // =====================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Invalid organization."
        )]
        public int? nOrgID { get; set; }


        // =====================================================
        // STATUS
        // =====================================================

        public bool? nBit { get; set; }

        public bool? nSABit { get; set; }
    }
}