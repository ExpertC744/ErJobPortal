using System;
using System.Collections.Generic;

namespace ErJobPortal.Models
{
    public class TPOSendNotificationM
    {
        public int NotificationID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public DateTime? NotificationDate { get; set; }

        public TimeSpan? NotificationTime { get; set; }

        public string Content { get; set; } = string.Empty;


        // =====================================================
        // ATTACHMENTS
        // =====================================================

        public string? Attachment1OriginalName { get; set; }

        public string? Attachment1Path { get; set; }

        public string? Attachment2OriginalName { get; set; }

        public string? Attachment2Path { get; set; }


        // =====================================================
        // TPO INFORMATION
        // =====================================================

        public int TPOID { get; set; }

        public string? CollegeCode { get; set; }


        // =====================================================
        // FILTERS
        // =====================================================

        public int? DepartmentID { get; set; }

        public int? BranchID { get; set; }

        public string? SearchText { get; set; }


        // =====================================================
        // SELECTED CANDIDATES
        // =====================================================

        public List<int> SelectedCandidateIDs { get; set; }
            = new List<int>();


        // =====================================================
        // STUDENTS
        // =====================================================

        public List<TPOTraineeInfoM> Students { get; set; }
            = new List<TPOTraineeInfoM>();
    }
}