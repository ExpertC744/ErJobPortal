namespace ErJobPortal.Models
{
    public class MyInternshipViewModel
    {
        // ==========================================
        // POST ID
        // ==========================================
        public int PostID { get; set; }

        // ==========================================
        // ORGANIZATION
        // ==========================================
        public int OrgID { get; set; }

        public string? OrganizationName { get; set; }

        // ==========================================
        // POST
        // ==========================================
        public string? Position { get; set; }

        public string? Location { get; set; }

        public string? InternshipFellowshipType { get; set; }

        public string? Facilities { get; set; }

        public string? ApplyStatus { get; set; }
    }
}