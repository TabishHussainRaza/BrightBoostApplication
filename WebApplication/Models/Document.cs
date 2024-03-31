namespace WebApplication.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string DocumentPath { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
