namespace ErJobPortal.Models
{
    public class OrgProfileListM
    {
        public int SrNo { get; set; }

        public int nID { get; set; }

        public int nOrgID { get; set; }

        public DateTime? ProfileCreationDate { get; set; }

        public string? OrganizationName { get; set; }

        public string? OrganizationLogo { get; set; }

        public string? OrganizationAddress { get; set; }

        public string? VerificationEmail { get; set; }

        public string? GSTNo { get; set; }

        public string? CINNo { get; set; }

        public string? CurrentEmployeeStrength { get; set; }

        public DateTime? DOB { get; set; }

        public int? EstablishmentYear { get; set; }

        // Organization Profile Status
        public bool nBit { get; set; }
    }
}
