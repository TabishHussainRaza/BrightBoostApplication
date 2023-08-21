namespace BrightBoostApplication.Models
{
    public class Attendance
    {
        public int id { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updateDate { get; set; }
        public Nullable<bool> status { get; set; }
        public int order { get; set; }
        public int fkId { get; set; }
        public StudentSignUp StudentSignUp { get; set; }
    }
}
