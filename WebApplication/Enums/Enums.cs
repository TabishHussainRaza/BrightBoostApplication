using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplication.Enums
{
    public class Enums
    {
        public enum Status
        {
            [Display(Name = "Order Placed")]
            Started = 1,

            [Display(Name = "Processing")]
            Processing,

            [Display(Name = "Shipped")]
            Shipped,

            [Display(Name = "Delievered")]
            Delivered,

            [Display(Name = "Paid")]
            Paid,

            [Display(Name = "Complete")]
            Complete,
        }
    }
}
