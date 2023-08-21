namespace BrightBoostApplication.Models
{
    public class Term
    {
        public int Id { get; set; }
        public string TermName { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updateDate { get; set; }
        public Nullable<bool> isActive { get; set; }
        public IList<TermCourse> TermCourse { get; set; }
    }
}
