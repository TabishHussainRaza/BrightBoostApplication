namespace WebApplication.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string FormattedAddress { get; set; }
        public Decimal Latitude { get; set; }
        public Decimal Longitude { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public int ContactPhone { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
        public IList<Staff> Staff { get; set; } = new List<Staff>();
        public int ReatilerId { get; set; }
        public Retailer Retailer { get; set; }
    }
}
