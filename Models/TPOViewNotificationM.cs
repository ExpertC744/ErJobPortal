namespace ErJobPortal.Models
{
    public class TPOViewNotificationM
    {
        public int NotificationID { get; set; }

        public string Title { get; set; } = "";

        public string Type { get; set; } = "";

        public string Audience { get; set; } = "";

        public DateTime? NotificationDate { get; set; }

        public TimeSpan? NotificationTime { get; set; }

        public string Content { get; set; } = "";

        public bool SendEmailNotification { get; set; }


        // ==========================================
        // ATTACHMENT 1
        // ==========================================

        public string? Attachment1OriginalName { get; set; }

        public string? Attachment1FileName { get; set; }

        public string? Attachment1Path { get; set; }

        public string? Attachment1ContentType { get; set; }

        public long? Attachment1Size { get; set; }


        // ==========================================
        // ATTACHMENT 2
        // ==========================================

        public string? Attachment2OriginalName { get; set; }

        public string? Attachment2FileName { get; set; }

        public string? Attachment2Path { get; set; }

        public string? Attachment2ContentType { get; set; }

        public long? Attachment2Size { get; set; }


        // ==========================================
        // STATUS
        // ==========================================

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public string Status { get; set; } = "";


        // ==========================================
        // AUDIT
        // ==========================================

        public DateTime CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public string CreatedByName { get; set; } = "";


        // =====================================================
        // NOTIFICATION TRACKING
        // =====================================================

        public int TotalSent { get; set; }

        public int TotalRead { get; set; }

        public int TotalUnread { get; set; }

        public IFormFile? AttachFile1 { get; set; }

        public IFormFile? AttachFile2 { get; set; }

        //public DateTime? NotificationDate { get; set; }

        //public TimeSpan? NotificationTime { get; set; }

        
    }
}