namespace BrightBoostApplication.Models
{
    public class TermCourse
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int TermId { get; set; }
        public Term Term { get; set; }
        public string Title { get; set; }
        public IList<StudentCourseSignUp> StudentCourseSignUp { get; set; }
    }
}
