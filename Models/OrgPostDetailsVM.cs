namespace ErJobPortal.Models
{
    public class OrgPostDetailsVM
    {
        public OrgPostM Post { get; set; } = new OrgPostM();

        public int CurrentIndex { get; set; }

        public int TotalRecords { get; set; }

        public int? PreviousId { get; set; }

        public int? NextId { get; set; }

        public bool HasPrevious => PreviousId.HasValue;

        public bool HasNext => NextId.HasValue;
    }
}
