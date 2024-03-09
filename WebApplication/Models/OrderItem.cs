namespace WebApplication.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int InventoryId { get; set; }
        public  Inventory Inventory { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
