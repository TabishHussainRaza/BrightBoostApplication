namespace BrightBoostApplication.Models
{
    public class TutorAllocation
    {
        public int Id { get; set; }
        public int TutorId { get; set; }
        public Tutor Tutor { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}
