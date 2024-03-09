using System.Text.RegularExpressions;

namespace WebApplication.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public bool IsActive { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = new Branch();
    }
}
