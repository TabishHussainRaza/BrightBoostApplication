using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BrightBoostApplication.Models;
using BrightBoostApplication.BLL;
using AutoMapper;
using BrightBoostApplication.Models.ViewModel;

namespace Identity.Controllers
{
    public class UserController : Controller
    {
        private readonly User User;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            User = new User(userManager);
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllUsers()
        {
            return Json(User.GetAllUsers());
        }

        [HttpPost]
        public JsonResult Add(CreateUserViewModel model)
        {
            var user = User.Add(model);
            return Json(user.Result);
        }

        [HttpPost]
        public JsonResult Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = User.Edit(model);
                return Json(user.Result);
            }
            return Json(false);
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(string id)
        {
            var user = await User.Delete(id);
            return Json(user);
        }

        [HttpPost]
        public async Task<JsonResult> ChangeUserStatus(EditUserViewModel model)
        {
            var user = await User.ChangeUserStatus(model);
            return Json(user);
        }

    }
}