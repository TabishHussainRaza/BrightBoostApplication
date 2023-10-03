namespace BrightBoostApplication.Models
{
    public class StudentCourseSignUp
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int termCourseId { get; set; }
        public TermCourse TermCourse { get; set; }
        public DateTime? createDate { get; set; }
    }
}
