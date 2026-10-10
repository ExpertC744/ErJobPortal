
using ErJobPortal.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ErJobPortal.Models
{
    // =========================================
    // GENERAL ACTIVITY MODEL
    // =========================================
    public class TPOSchedulerActivityM
    {
        public int ActivityID { get; set; }

        public int TPOID { get; set; }

        [Required(ErrorMessage = "Please enter activity title.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter description.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select start date.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please select end date.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Please select start time.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Please select end time.")]
        public TimeSpan EndTime { get; set; }

        [StringLength(300)]
        public string? Location { get; set; }

        [Required]
        [StringLength(50)]
        public string Participants { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Reminder { get; set; }

        [StringLength(30)]
        public string? ReminderChannel { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Upcoming";

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // =========================================
    // PLACEMENT DRIVE MODEL
    // =========================================
    public class TPOSchedulerPlacementDriveM
    {
        public int PlacementDriveID { get; set; }

        public int TPOID { get; set; }

        [Required(ErrorMessage = "Please enter company name.")]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter job position.")]
        [StringLength(200)]
        public string JobPosition { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select job type.")]
        [StringLength(50)]
        public string JobType { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Salary { get; set; }

        [Required(ErrorMessage = "Please enter job location.")]
        [StringLength(300)]
        public string JobLocation { get; set; } = string.Empty;

        public string? RequiredSkills { get; set; }

        [Required(ErrorMessage = "Please enter job description.")]
        public string JobDescription { get; set; } = string.Empty;

        [StringLength(300)]
        public string? EligibleCourse { get; set; }

        [Range(0, 100)]
        public decimal? TenthPercentage { get; set; }

        [Range(0, 100)]
        public decimal? TwelfthPercentage { get; set; }

        [Range(0, 100)]
        public decimal? MinimumPercentage { get; set; }

        [Range(1900, 2200)]
        public int? GraduationYear { get; set; }

        public string? EligibilityCriteria { get; set; }

        [Required(ErrorMessage = "Please select start date.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please select end date.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [StringLength(300)]
        public string? Venue { get; set; }

        [StringLength(150)]
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid contact email.")]
        [StringLength(254)]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Please enter a valid contact phone.")]
        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [StringLength(50)]
        public string? Participants { get; set; }

        [Required]
        [StringLength(30)]
        public string PublicationStatus { get; set; } = "Draft";

        public string? Instructions { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class TPOSchedulerPageM
    {
        public string ActivityType { get; set; } = "General";

        public TPOSchedulerActivityM General { get; set; }
            = new TPOSchedulerActivityM();

        public TPOSchedulerPlacementDriveM Placement { get; set; }
            = new TPOSchedulerPlacementDriveM();
    }
}

