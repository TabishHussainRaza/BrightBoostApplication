using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BrightBoostApplication.Models;
using BrightBoostApplication.BLL;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private Role Role;
        public RoleController(RoleManager<IdentityRole> roleMgr, UserManager<ApplicationUser> userMrg, IMapper mapper)
        {
            Role = new Role(roleMgr, userMrg, mapper);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var condition = Role.Create(name);
                return Json(condition.Result);
            }
            return Json(false);
        }

        [HttpGet]
        public JsonResult GetAllRoles()
        {
            var roles = Role.GetRolesAllAsync();
            return Json(roles.Result);
        }

        [HttpDelete]
        public JsonResult DeleteRole(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                var roles = Role.DeleteRole(id);
                return Json(roles.Result);
            }
            return Json(false);
        }

        [HttpPost]
        public JsonResult UpdateRole(string id, string name)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(id))
            {
                var condition = Role.UpdateRole(id, name);
                return Json(condition.Result);
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<JsonResult> AddRoleMapping(RoleAssignModel RoleAssignModel)
        {
            var condition = await Role.AddRoleMapping(RoleAssignModel);
            return Json(condition);
        }

        public async Task<JsonResult> GetMyRoles(string userId)
        {
            var roles = Role.GetMyRoles(userId);
            return Json(roles);
        }

    }
}