namespace BrightBoostApplication.Models
{
    public class StudentSignUp
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}
