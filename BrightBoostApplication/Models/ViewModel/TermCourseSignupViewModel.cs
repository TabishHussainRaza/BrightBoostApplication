namespace BrightBoostApplication.Models.ViewModel;

public class TermCourseSignUpViewModel
{
    public int StudentId { get; set; }
    public List<int> SubjectIds { get; set; }
    public bool RemoveAll { get; set; }
}