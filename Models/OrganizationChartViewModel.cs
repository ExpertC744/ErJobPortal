namespace ErJobPortal.Models
{
    public class OrganizationChartViewModel
    {
        // =====================================================
        // AREA CHART
        // =====================================================

        public int PostYear { get; set; }

        public int PostMonth { get; set; }

        public int TotalEligibleTrainees { get; set; }


        // =====================================================
        // CANDIDATE REGISTRATION CHART
        // tblcandidateRegister -> RegDate
        // =====================================================

        public int RegistrationYear { get; set; }

        public int RegistrationMonth { get; set; }

        public int TotalRegistrations { get; set; }
    }


    public class OrganizationChartsViewModel
    {
        public List<OrganizationChartViewModel> AreaChartData { get; set; }
            = new List<OrganizationChartViewModel>();

        public List<OrganizationChartViewModel> BarChartData { get; set; }
            = new List<OrganizationChartViewModel>();
    }
}