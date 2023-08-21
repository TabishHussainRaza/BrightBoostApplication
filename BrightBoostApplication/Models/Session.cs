namespace BrightBoostApplication.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string SessionName { get; set; }
        public string SessionDay { get; set; }
        public string SessionVenue { get; set; }
        public string SessionColor { get; set; }
        public DateTime startTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updateDate { get; set; }
        public Nullable<bool> isActive { get; set; }
        public int fkId { get; set; }
        public TermCourse TermCourse { get; set; }
        public IList<TutorAllocation> TutorAllocation { get; set; }
        public IList<StudentSignUp> StudentSignUp { get; set; }
    }
}
