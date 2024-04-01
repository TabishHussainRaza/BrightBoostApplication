namespace WebApplication.Models.ViewModel
{
    public class ProductVM: Product
    {
        public int category { get; set; }
        public IList<IFormFile> files { get; set; }
    }
}
