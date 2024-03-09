using System.Text.RegularExpressions;

namespace WebApplication.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
        public IList<Order> Orders { get; set; } = new List<Order>();
        public IList<Address> Address { get; set; } = new List<Address>();
    }
}
