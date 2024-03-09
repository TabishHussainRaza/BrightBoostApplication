using System.Text.RegularExpressions;

namespace WebApplication.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string FormattedAddress { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Decimal Latitude { get; set; }
        public Decimal Longitude { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
