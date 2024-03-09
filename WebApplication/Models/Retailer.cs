namespace WebApplication.Models
{
    public class Retailer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
        public IList<Branch> Branches { get; set; } = new List<Branch>();
    }
}
