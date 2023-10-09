namespace BrightBoostApplication.Models.ViewModel;

public class TutorQuestionCreateViewModel
{
    public int TutorAllocationId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Answer { get; set; }
    public string Date { get; set; }
}