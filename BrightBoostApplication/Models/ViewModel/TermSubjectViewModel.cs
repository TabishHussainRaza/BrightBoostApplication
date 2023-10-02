namespace BrightBoostApplication.Models.ViewModel
{
    public class TermSubjectViewModel : Subject
    {
        public int TermCourseId { get; set; }
        public int? StuId { get; set; }
        public int? TutorId { get; set; }
    }
}

