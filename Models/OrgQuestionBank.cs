
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ErJobPortal.Models
{
    public class OrgQuestionBank
    {
        public int QuestionBankID { get; set; }

        public int OrgID { get; set; }

        [Required(ErrorMessage = "Question bank title is required.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a stream/domain.")]
        [StringLength(100)]
        public string StreamDomain { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Course { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(150)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Topics are required.")]
        [StringLength(1000)]
        public string Topics { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        // Uploaded file
        public IFormFile? QuestionFile { get; set; }

        public string? OriginalFileName { get; set; }
        public string? StoredFileName { get; set; }
        public string? FileExtension { get; set; }
        public string? ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public string? FilePath { get; set; }

        // Access settings
        public string AudienceType { get; set; } = "AllCandidates";

        public bool IsPublished { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
