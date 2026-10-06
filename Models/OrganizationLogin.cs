namespace ErJobPortal.Models
{
    public class OrganizationLogin
    {
        public string? sEmail { get; set; }
        public string? sPassword { get; set; }

        public bool IsDisabledBySuperAdmin { get; set; }
    }
}

