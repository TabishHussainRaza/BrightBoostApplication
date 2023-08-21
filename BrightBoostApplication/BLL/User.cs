using AutoMapper;
using BrightBoostApplication.Models;
using BrightBoostApplication.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;

namespace BrightBoostApplication.BLL
{
    public class User
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public User(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            IEnumerable<ApplicationUser> users = _userManager.Users;
            return users;
        }

        public async Task<bool> Add(CreateUserViewModel model)
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
                return true;
            }
            return false;
        }

        public async Task<bool> Edit(EditUserViewModel model)
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
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ChangeUserStatus(EditUserViewModel model)
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
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
