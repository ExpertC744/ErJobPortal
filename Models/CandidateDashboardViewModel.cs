namespace ErJobPortal.Models
{
    public class CandidateDashboardViewModel
    {
        public int CandidateID { get; set; }

        // Suitable openings graph
        public List<string> SuitableOpeningMonths { get; set; } = new List<string>();

        public List<int> SuitableOpeningCounts { get; set; } = new List<int>();
    }
}
