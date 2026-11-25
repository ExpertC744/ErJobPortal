
//page by anam
namespace ErJobPortal.Models
{
    public class OrganizationStatisticsVM
    {
        public List<ChartItemVM> TopSkills { get; set; }
            = new List<ChartItemVM>();

        public List<ChartItemVM> TopCities { get; set; }
            = new List<ChartItemVM>();

        public List<ChartItemVM> TopFields { get; set; }
            = new List<ChartItemVM>();
    }

    public class ChartItemVM
    {
        public string Name { get; set; } = "";

        public int Count { get; set; }

        public double Percentage { get; set; }
    }
}
