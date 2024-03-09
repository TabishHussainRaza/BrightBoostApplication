namespace WebApplication.Models
{
    public class CategoryType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
        public IList<CategorySubType> CategorySubTypes { get; set; } = new List<CategorySubType> ();
    }
}
