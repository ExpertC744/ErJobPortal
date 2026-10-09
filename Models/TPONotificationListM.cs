namespace ErJobPortal.Models
{
    public class TPONotificationListM
    {
        // ============================
        // Notification Information
        // ============================

        public int NotificationID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;


        // ============================
        // Date & Time
        // ============================

        public DateTime? NotificationDate { get; set; }

        public TimeSpan? NotificationTime { get; set; }


        // Display format
        // Example: 08/10/2026 10:00 AM

        public string DisplayDateTime { get; set; } = string.Empty;


        // ============================
        // Status
        // ============================

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string Status { get; set; } = string.Empty;


        // ============================
        // Content
        // ============================

        public string Content { get; set; } = string.Empty;


        // ============================
        // Email
        // ============================

        public bool SendEmailNotification { get; set; }


        // ============================
        // Attachment 1
        // ============================

        public string? Attachment1OriginalName { get; set; }

        public string? Attachment1Path { get; set; }


        // ============================
        // Attachment 2
        // ============================

        public string? Attachment2OriginalName { get; set; }

        public string? Attachment2Path { get; set; }


        // ============================
        // Created By
        // ============================

        public int? CreatedBy { get; set; }

        public string? CreatedByName { get; set; }

        public DateTime CreatedDate { get; set; }


        // =====================================================
        // NOTIFICATION TRACKING
        // =====================================================

        public int TotalSent { get; set; }

        public int TotalRead { get; set; }

        public int TotalUnread { get; set; }
    }
}