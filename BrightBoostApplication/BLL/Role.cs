using AutoMapper;
using BrightBoostApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace BrightBoostApplication.BLL
{
    public class Role
    {
        private static RoleManager<IdentityRole>? roleManager;
        private static UserManager<ApplicationUser>? userManager;
        private static IMapper? _mapper;
        public Role(RoleManager<IdentityRole> roleMgr, UserManager<ApplicationUser> userMrg, IMapper mapper)
        {
            roleManager = roleMgr;
            userManager = userMrg;
            _mapper = mapper;
        }

        public static async Task<RoleEdit> UpdateById(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            List<ApplicationUser> members = new List<ApplicationUser>();
            List<ApplicationUser> nonMembers = new List<ApplicationUser>();
            foreach (ApplicationUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }

            RoleEdit roleEdit = new()
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            };

            return roleEdit;
        }

        public async Task<bool> Update(RoleModification model)
        {
            IdentityResult result;

            foreach (string userId in model.AddIds ?? new string[] { })
            {
                ApplicationUser user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    
                    try
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        throw ex;
                    }
                    if (!result.Succeeded)
                    {
                        return false;
                    }
                        
                }
            }
            return true;
            
        }


        public static async Task<IEnumerable<RoleModel>> GetRolesAllAsync()
        {
            var roles = _mapper.Map<IEnumerable<RoleModel>>(roleManager.Roles);

            foreach (var role in roles)
            {
                int userCounter = 0;
                foreach (var user in userManager.Users)
                {
                    if (user != null && await userManager.IsInRoleAsync(user, role.Name))
                        userCounter++;
                }
                role.userCount = userCounter == 0 ? "No Users" : userCounter.ToString();
            }

            return roles;
        }

        public static async Task<bool> Create(string name)
        {
            IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            IdentityResult result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> UpdateRole(string id, string name)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if(role != null)
            {
                role.Name = name;
                role.NormalizedName = name.ToUpper();
                IdentityResult result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddRoleMapping(RoleAssignModel RoleAssignModel)
        {
            ApplicationUser user = await userManager.FindByIdAsync(RoleAssignModel.userId);
            if (user != null)
            {
                if (RoleAssignModel.removeAll != null && RoleAssignModel.removeAll == true)
                {
                    var existingMappings = await userManager.GetRolesAsync(user);
                    var result = await userManager.RemoveFromRolesAsync(user, existingMappings);

                    return true;
                }
                else if (RoleAssignModel.RoleIds != null && RoleAssignModel.RoleIds.Length > 0)
                {
                    List<IdentityResult> result = new();
                    var existingMappings = await userManager.GetRolesAsync(user);
                    HashSet<IdentityRole> roles = new HashSet<IdentityRole>();

                    foreach (var name in existingMappings)
                    {
                        var roleDetails = await roleManager.FindByNameAsync(name);
                        roles.Add(roleDetails);
                    }

                    var ids = roles.Select(i => i.Id);
                    var rolesToAdd = RoleAssignModel.RoleIds.Where(id => !ids.Contains(id)).ToList();

                    foreach (var id in rolesToAdd)
                    {
                        var roleDetails = await roleManager.FindByIdAsync(id);
                        var currResult = await userManager.AddToRoleAsync(user, roleDetails.Name);
                        result.Add(currResult);
                    }

                    var groupsToRemove = ids.Except(RoleAssignModel.RoleIds);
                    if (groupsToRemove.Count() > 0)
                    {
                        foreach (var id in groupsToRemove)
                        {
                            var roleDetails = await roleManager.FindByIdAsync(id);
                            var currResult = await userManager.RemoveFromRoleAsync(user, roleDetails.Name);
                            result.Add(currResult);
                        }
                    }

                    if (result.Where(ir => !(ir.Succeeded)).Count() > 0)
                    {
                        return false;
                    }
                    return true;
                }
            }

            return false;
        }

        public async Task<List<IdentityRole>> GetMyRoles(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            List<IdentityRole> roles = new List<IdentityRole>();
            if (user != null)
            {
                
                foreach (IdentityRole role in roleManager.Roles)
                {
                    var condition = await userManager.IsInRoleAsync(user, role.Name);
                    if (condition)
                    {
                        roles.Add(role);
                    }
                }
            }
            return roles.ToList();
        }

    }
}
