namespace BrightBoostApplication.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updateDate { get; set; }
        public Nullable<bool> isActive { get; set; }
        public IList<StudentSignUp> StudentSignUp { get; set; }
        public IList<StudentCourseSignUp> StudentCourseSignUp { get; set; }
    }
}
