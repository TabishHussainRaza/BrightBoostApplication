namespace BrightBoostApplication.Models.ViewModel
{

    public class SessionViewModel
    {
        public int Id { get; set; }
        public string SessionName { get; set; }
        public string SessionDay { get; set; }
        public string SessionVenue { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? MaxNumber { get; set; }
    }
}
