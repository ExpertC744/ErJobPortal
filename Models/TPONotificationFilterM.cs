namespace ErJobPortal.Models
{
    public class TPONotificationFilterM
    {
        public string? Type { get; set; }

        public string? Audience { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}