using System;

namespace ErJobPortal.Models
{
    public class CandidateNotificationM
    {
        public int RecipientID { get; set; }

        public int NotificationID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public DateTime? NotificationDate { get; set; }

        public TimeSpan? NotificationTime { get; set; }

        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime SentDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? Attachment1OriginalName { get; set; }

        public string? Attachment1Path { get; set; }

        public string? Attachment2OriginalName { get; set; }

        public string? Attachment2Path { get; set; }
    }
}