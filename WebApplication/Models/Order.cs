namespace WebApplication.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem> ();
        public int AddressId { get; set; }
        public Address Address { get; set; }
        
    }
}
