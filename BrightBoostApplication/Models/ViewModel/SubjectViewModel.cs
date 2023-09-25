namespace BrightBoostApplication.Models.ViewModel
{
    public class SubjectViewModel
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string SubjectDescription { get; set; }
    }

    public class SubjectAssignModel
    {
        public int[]? SubjectIds { get; set; }
        public int TermId { get; set; }
        public bool? removeAll { get; set; }
    }
}