namespace ErJobPortal.Models
{
    public class OrganizationProfileDetailsVM
    {
        // Current organization
        public OrgProfileListM Organization { get; set; } = new OrgProfileListM();

        // Navigation
        public int CurrentIndex { get; set; }
        public int TotalRecords { get; set; }

        public int? PreviousId { get; set; }
        public int? NextId { get; set; }

        public bool HasPrevious => PreviousId.HasValue;
        public bool HasNext => NextId.HasValue;
    }
}
