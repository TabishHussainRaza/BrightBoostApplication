using System.Text.RegularExpressions;

namespace WebApplication.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int TransactionStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
