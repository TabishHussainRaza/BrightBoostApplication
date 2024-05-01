namespace WebApplication.Models.ViewModel
{
    public class OrderVM: Order
    {
        public string Email { get; set; }
        public string FormattedAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<OrderProduct> OrderItems { get; set; } = new List<OrderProduct> ();
    }

    public class OrderProduct
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Product ProductDetails { get; set; }
    }
}
