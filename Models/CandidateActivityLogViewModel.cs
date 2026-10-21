
namespace ErJobPortal.Models
{
    public class CandidateActivityLogViewModel
    {
        public long VisitID { get; set; }

        public string? IPAddress { get; set; }
        public string? ISP { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? TimeZone { get; set; }

        public string? BrowserName { get; set; }
        public string? BrowserVersion { get; set; }
        public string? OperatingSystem { get; set; }
        public string? DeviceType { get; set; }

        public string? RequestPath { get; set; }
        public string? PageName { get; set; }
        public string? ReferrerUrl { get; set; }

        public DateTime VisitDateTime { get; set; }

        public bool IsLoggedIn { get; set; }
    }
}
