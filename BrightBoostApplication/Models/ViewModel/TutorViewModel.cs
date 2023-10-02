namespace BrightBoostApplication.Models.ViewModel
{
    public class TutorViewModel
    {
        public int TutorId { get; set; }
        public string Name { get; set; }
    }

    public class TutorCRUDViewModel
    {
        public List<int> TutorId { get; set; }
        public int SessionId { get; set; }
        public bool RemoveAll { get; set; }
    }
}

