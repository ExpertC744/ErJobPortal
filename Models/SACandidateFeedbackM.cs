namespace ErJobPortal.Models
{
    public class SACandidateFeedbackM
    {
        public int nID { get; set; }

        // Actual Feedback ID
        public int FeedbackID { get; set; }
        public string? sFName { get; set; }

        public string? sLName { get; set; }

        public int? nAdminID { get; set; }

        public int? nSABit { get; set; }

        public string? Status { get; set; }
    }
}