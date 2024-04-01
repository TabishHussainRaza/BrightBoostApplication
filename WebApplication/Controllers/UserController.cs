using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApplication.Models;
using AutoMapper;
using WebApplication.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
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
            //Main Group User Management
            var user = new ApplicationUser
            {
                firstName = model.firstName,
                lastName = model.lastName,
                UserName = model.Email,
                Email = model.Email,
                isActive = true,
                GroupId = 1
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<JsonResult> SaveAsync([FromBody] SaveUserViewModel model)
        {
            if(model == null)
            {
                return Json(false);
            }

            if (string.IsNullOrWhiteSpace(model.userId))
            {
                var newUser = new ApplicationUser
                {
                    firstName = model.firstName,
                    lastName = model.lastName,
                    UserName = model.email,
                    Email = model.email,
                    isActive = true,
                    GroupId = model.groupId
                };
                var result = await _userManager.CreateAsync(newUser, model.password);
                if (result.Succeeded)
                {
                    return Json(true);
                }
                return Json(false);
            }

            var user = await _userManager.FindByIdAsync(model.userId);
            if (user != null)
            {
                user.UserName = model.email;
                user.Email = model.email;
                user.firstName = model.firstName;
                user.lastName = model.lastName;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Json(true);
                }
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

        [HttpGet]
        public async Task<JsonResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return Json(user);
        }
    }
}