
namespace ErJobPortal.Models
{
    public class SAChartData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalCount { get; set; }
    }

    public class SAChartsViewModel
    {
        public List<SAChartData> CandidateData { get; set; }
            = new List<SAChartData>();

        public List<SAChartData> OrganizationData { get; set; }
            = new List<SAChartData>();

        public List<SAChartData> InternshipPostData { get; set; }
            = new List<SAChartData>();
    }

    //anam
    public class SAChartItemM
    {
        public string Name { get; set; } = string.Empty;

        public decimal Percentage { get; set; }
    }

    public class SATopStatisticsVM
    {
        public List<SAChartItemM> TopSkills { get; set; }
            = new List<SAChartItemM>();

        public List<SAChartItemM> TopCities { get; set; }
            = new List<SAChartItemM>();

        public List<SAChartItemM> TopFields { get; set; }
            = new List<SAChartItemM>();
    }
}




