namespace BrightBoostApplication.Models
{
    public class Question
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string answer { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updateDate { get; set; }
        public Nullable<bool> status { get; set; }
        public int order { get; set; }
        public int fkId { get; set; }
        public StudentSignUp StudentSignUp { get; set; }
    }
}
