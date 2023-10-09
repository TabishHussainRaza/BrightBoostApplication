namespace BrightBoostApplication.Models.ViewModel
{
    public class AttendanceListVM
    {
        public DateTime date { get; set; }
        public int sessionId { get; set; }
        public List<AttendanceVM> studentData { get; set; }
    }
    public class AttendanceVM
    {
        public int studentId { get; set; }
        public bool attendanceStatus { get; set; }
    }
}