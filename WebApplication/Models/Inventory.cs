namespace WebApplication.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Quantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
