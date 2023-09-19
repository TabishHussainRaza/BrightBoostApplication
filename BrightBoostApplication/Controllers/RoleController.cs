using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BrightBoostApplication.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole>? roleManager;
        private UserManager<ApplicationUser>? userManager;
        private IMapper? _mapper;
        public RoleController(RoleManager<IdentityRole> roleMgr, UserManager<ApplicationUser> userMrg, IMapper mapper)
        {
            roleManager = roleMgr;
            userManager = userMrg;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateAsync(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllRolesAsync()
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
            return Json(roles);
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteRoleAsync(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                IdentityRole role = await roleManager.FindByIdAsync(id);
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateRoleAsync(string id, string name)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(id))
            {
                IdentityRole role = await roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    role.Name = name;
                    role.NormalizedName = name.ToUpper();
                    IdentityResult result = await roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return Json(true);
                    }
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<JsonResult> AddRoleMapping(RoleAssignModel RoleAssignModel)
        {
            ApplicationUser user = await userManager.FindByIdAsync(RoleAssignModel.userId);
            if (user != null)
            {
                if (RoleAssignModel.removeAll != null && RoleAssignModel.removeAll == true)
                {
                    var existingMappings = await userManager.GetRolesAsync(user);
                    var result = await userManager.RemoveFromRolesAsync(user, existingMappings);

                    return Json(true);
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
                        return Json(false);
                    }
                    return Json(true);
                }
            }

            return Json(false);
        }

        public async Task<JsonResult> GetMyRoles(string userId)
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
            return Json(roles);
        }

        public async Task<JsonResult> GetMyUserRoles(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                List<IdentityRole> roles = new List<IdentityRole>();
                foreach (IdentityRole role in roleManager.Roles)
                {
                    var condition = await userManager.IsInRoleAsync(user, role.Name);
                    if (condition)
                    {
                        roles.Add(role);
                    }
                }
                return Json(roles.ToList());
            }
            return Json(false);
        }

    }
}