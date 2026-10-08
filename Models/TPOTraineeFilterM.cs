namespace ErJobPortal.Models
{
    public class TPOTraineeFilterM
    {
        public int CollegeId { get; set; }

        public string? SearchText { get; set; }

        public int? BranchId { get; set; }

        public int? CurrentYear { get; set; }

        public int? AdmissionYear { get; set; }

        public int? PassoutYear { get; set; }

        public int? Gender { get; set; }
    }
}