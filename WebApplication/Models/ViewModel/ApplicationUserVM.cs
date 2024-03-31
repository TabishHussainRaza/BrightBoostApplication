namespace WebApplication.Models.ViewModel
{
    public class ApplicationUserVM
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Nullable<bool> isActive { get; set; }
    }

    public class CreateUserViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int? groupId { get; set; }
    }

    public class SaveUserViewModel
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int? groupId { get; set; }
    }

    public class EditUserViewModel
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public Nullable<bool> isActive { get; set; }
        public int? groupId { get; set; }
    }
}
