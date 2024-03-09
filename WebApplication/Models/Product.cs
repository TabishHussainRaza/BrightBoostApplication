namespace WebApplication.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public Decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int CategorySubTypeId { get; set; }
        public CategorySubType CategorySubType { get; set; }
    }
}
