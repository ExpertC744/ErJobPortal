namespace ErJobPortal.Models
{
    public class CandidateChartsViewModel
    {
        public List<CandidateChartItem> TopSkills { get; set; } = new();

        public List<CandidateChartItem> TopCities { get; set; } = new();

        public List<CandidateChartItem> TopFields { get; set; } = new();
    }
}