namespace WebApplication.Models
{
    public class CategorySubType
    {
        public int Id { get; set; }
        public string SubTypeName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
        public int ProductTypeId { get; set; }
        public CategoryType ProductType { get; set; }
    }
}
