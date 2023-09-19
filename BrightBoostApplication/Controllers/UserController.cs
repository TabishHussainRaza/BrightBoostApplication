using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BrightBoostApplication.Models;
using AutoMapper;
using BrightBoostApplication.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllUsers()
        {
            return Json(_userManager.Users.ToList());
        }

        [HttpPost]
        public async Task<JsonResult> AddAsync(CreateUserViewModel model)
        {
            var user = new ApplicationUser
            {
                firstName = model.firstName,
                lastName = model.lastName,
                UserName = model.Email,
                Email = model.Email,
                isActive = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<JsonResult> EditAsync(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.id);
                if (user != null)
                {
                    user.UserName = model.email;
                    user.Email = model.email;
                    user.firstName = model.firstName;
                    user.lastName = model.lastName;
                    user.isActive = model.isActive;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Json(true);
                    }
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<JsonResult> ChangeUserStatus(EditUserViewModel model)
        {
            if (model.isActive != null && model.id != null)
            {
                var user = await _userManager.FindByIdAsync(model.id);
                if (user != null)
                {
                    user.isActive = model.isActive;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Json(true);
                    }
                }
            }
            return Json(false);
        }

    }
}