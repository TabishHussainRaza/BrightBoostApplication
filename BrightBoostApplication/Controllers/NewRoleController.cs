using AutoMapper.Execution;
using BrightBoostApplication.BLL;
using BrightBoostApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BrightBoostApplication.Controllers
{
    public class NewRoleController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;

        public NewRoleController(RoleManager<IdentityRole> roleMgr, UserManager<ApplicationUser> userMrg)
        {
            roleManager = roleMgr;
            userManager = userMrg;
        }

        // other methods

        public async Task<JsonResult> GetMyRoles(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if(user != null)
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

        public async Task<IActionResult> Update(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            List<ApplicationUser> members = new List<ApplicationUser>();
            List<ApplicationUser> nonMembers = new List<ApplicationUser>();
            foreach (ApplicationUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View( new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    ApplicationUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            return await Update(model.RoleId);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    ApplicationUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            return await Update(model.RoleId);
                    }
                }
            }

            if (ModelState.IsValid)
                return View("Index");
            else
                return await Update(model.RoleId);
        }

        
    }
}
