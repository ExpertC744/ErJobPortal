
namespace ErJobPortal.Models
{
    public class TPOSchedulerReminderM
    {
        public int ActivityID { get; set; }
        public int TPOID { get; set; }

        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Location { get; set; }

        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }

        public string? Reminder { get; set; }
        public string? ReminderChannel { get; set; }

        public string? TPOName { get; set; }
        public string? TPOEmail { get; set; }

        public DateTime ActivityStart { get; set; }
        public DateTime ReminderDueAt { get; set; }
    }
}
