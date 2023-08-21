namespace BrightBoostApplication.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string subjectName { get; set; }
        public string subjectDescription { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updateDate { get; set; }
        public Nullable<bool> isActive { get; set; }
        public IList<TermCourse> TermCourse { get; set; }

        public IList<Expertise> Expertise { get; set; }
    }
}
