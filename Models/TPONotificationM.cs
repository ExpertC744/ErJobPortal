using Microsoft.AspNetCore.Http;

namespace ErJobPortal.Models
{
    public class TPONotificationM
    {
        public int NotificationID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public DateTime? NotificationDate { get; set; }

        public TimeSpan? NotificationTime { get; set; }

        public string Content { get; set; } = string.Empty;

        public bool SendEmailNotification { get; set; }

        // ============================
        // Attachment 1
        // ============================

        public IFormFile? AttachFile1 { get; set; }

        public string? Attachment1OriginalName { get; set; }

        public string? Attachment1FileName { get; set; }

        public string? Attachment1Path { get; set; }

        public string? Attachment1ContentType { get; set; }

        public long? Attachment1Size { get; set; }


        // ============================
        // Attachment 2
        // ============================

        public IFormFile? AttachFile2 { get; set; }

        public string? Attachment2OriginalName { get; set; }

        public string? Attachment2FileName { get; set; }

        public string? Attachment2Path { get; set; }

        public string? Attachment2ContentType { get; set; }

        public long? Attachment2Size { get; set; }


        // ============================
        // Created By
        // ============================

        public int? CreatedBy { get; set; }

        public string? CreatedByName { get; set; }


        // ============================
        // Status
        // ============================

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}