using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BrightBoostApplication.Models
{
    public class RoleModel:IdentityRole
    {
        public string userCount { get; set; }
        public bool? userFlag { get; set; }
    }
    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }
    public class RoleModification
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }

        public string[]? AddIds { get; set; }

        public string[]? DeleteIds { get; set; }
    }
    public class RoleAssignModel
    {
        public string[]? RoleName { get; set; }

        public string[]? RoleIds { get; set; }

        public string userId { get; set; }
        public bool? removeAll { get; set; }
    }
}