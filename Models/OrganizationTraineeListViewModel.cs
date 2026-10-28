namespace ErJobPortal.Models
{
    public class OrganizationTraineeListViewModel
    {
        public int CandidateID { get; set; }

        public string CandidateName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public DateTime? ApplyDate { get; set; }

        public int PostID { get; set; }

        public DateTime? StartDate { get; set; }

        public string Status { get; set; }

        public string FinalStatus { get; set; }

        public string Comment { get; set; }
    }
}
