namespace WebApplication.Models.ViewModel
{
    public class ProductVM: Product
    {
        public int category { get; set; }
        public IList<IFormFile> files { get; set; }
    }

    public class UpdateVM 
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
