namespace BrightBoostApplication.Models
{
    public class Tutor
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public string Availability { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updateDate { get; set; }
        public Nullable<bool> isActive { get; set; }
        public IList<TutorAllocation> TutorAllocation { get; set; }
        public IList<Expertise> Expertise { get; set; }
    }
}
