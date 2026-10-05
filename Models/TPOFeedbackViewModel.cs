using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    public class TPOFeedbackViewModel
    {
        // =====================================================
        // FEEDBACK QUESTION ID
        // =====================================================

        public int nID { get; set; }


        // =====================================================
        // QUESTIONS
        // Loaded from tblSAOrgFeedback
        // =====================================================

        public string? Que1 { get; set; }

        public string? Que2 { get; set; }

        public string? Que3 { get; set; }

        public string? Que4 { get; set; }

        public string? Que5 { get; set; }


        // =====================================================
        // TPO ANSWERS
        // =====================================================

        [Required(ErrorMessage = "Please answer Question 1.")]
        public string? sQue1 { get; set; }


        [Required(ErrorMessage = "Please answer Question 2.")]
        public string? sQue2 { get; set; }


        [Required(ErrorMessage = "Please answer Question 3.")]
        public string? sQue3 { get; set; }


        [Required(ErrorMessage = "Please answer Question 4.")]
        public string? sQue4 { get; set; }


        public string? sQue5 { get; set; }


        // =====================================================
        // TPO INFORMATION
        // =====================================================

        public int TPOID { get; set; }


        // =====================================================
        // FLAGS
        // =====================================================

        public int nBit { get; set; }

        public int nSABit { get; set; }

        public int nTPO { get; set; }
    }
}