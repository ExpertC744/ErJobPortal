namespace ErJobPortal.Models
{
    public class HomeFeedbackCardM
    {
        public int CandidateID { get; set; }

        public string TraineeName { get; set; } = string.Empty;

        public string ProfileImage { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Feedback { get; set; } = string.Empty;

        public int Rating { get; set; }
    }
}
