namespace BrightBoostApplication.Models.ViewModel
{
    public class StatisticsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalCourseRegistrations { get; set; }
        public int TotalSessionRegistrations { get; set; }
        public List<CourseRegistrationViewModel> registrationData { get; set; }
        public List<QuestionStatViewModel> QuestionStat { get; set; }
    }

    public class CourseRegistrationViewModel
    {
        public string Month { get; set; }
        public int Registrations { get; set; }
    }

    public class QuestionStatViewModel
    {
        public string Month { get; set; }
        public int total { get; set; }
    }
}
