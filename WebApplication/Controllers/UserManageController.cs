using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebApplication.Data;
using WebApplication.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [ApiController]
    public class UserManageController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole>? _roleManager;
        private readonly ApplicationDbContext _context;

        public class InputModel
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Password { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UpdateModel
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class SaveAddressModel : Address
        {
            public string IserId { get; set; }
        }

        public UserManageController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleMgr, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleMgr;
            _context = context;
        }

        [HttpPost]
        [Route("api/UserManage/login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    return Ok(new { message = "Login successful", user_id = user.Id });
                }

                return Unauthorized(new { message = "Unauthorised" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error"});
            }
            
        }

        [HttpPost]
        [Route("api/UserManage/register")]
        public async Task<IActionResult> Register([FromBody] InputModel Input)
        {
            try
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, firstName = Input.FirstName, lastName = Input.LastName, isActive = true, PhoneNumber = Input.PhoneNumber };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var roleDetails = await _roleManager.FindByNameAsync("Customer");
                    if (roleDetails != null)
                    {
                        var currResult = await _userManager.AddToRoleAsync(user, roleDetails.Name);
                        if (currResult.Succeeded)
                        {
                            var customer = new Customer()
                            {
                                UserId = user.Id,
                                LastUpdateDateTime = DateTime.UtcNow,
                                CreatedDateTime = DateTime.UtcNow,
                                IsActive = true,
                            };

                            _context.Add(customer);
                            await _context.SaveChangesAsync();

                            return Ok(new { message = "Registeration Successful", user_id = user.Id });
                        }
                    }
                }
                return Unauthorized(new { message = "Unauthorised" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error" });
            }

        }

        [HttpGet]
        [Route("api/UserManage/details")]
        public async Task<IActionResult> Details([FromBody] UpdateModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    return Ok(new {email = user.Email, firstName = user.firstName, lastname=user.lastName, Phone = user.PhoneNumber });
                }
                return Unauthorized(new { message = "Unauthorised" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error" });
            }

        }

        [HttpPost]
        [Route("api/UserManage/updateDetails")]
        public async Task<IActionResult> UpdateDetails([FromBody] UpdateModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email; // Update email
                    user.firstName = model.FirstName; // Update first name
                    user.lastName = model.LastName; // Update last name
                    user.PhoneNumber = model.PhoneNumber; // Update phone number

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok(new { message = "User details updated successfully" });
                    }
                    else
                    {
                        // If updating user failed, return error messages
                        var errors = result.Errors.Select(e => e.Description);
                        return BadRequest(new { message = "Failed to update user details", errors = errors });
                    }
                }
                return Unauthorized(new { message = "Unauthorised" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error" });
            }

        }

        [HttpGet]
        [Route("api/UserManage/address")]
        public async Task<IActionResult> MyAddress([FromBody] UpdateModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var address = _context.Customers.Where(cus => cus.UserId == user.Id).Include(cus =>cus.Address).Select(add => new Address()
                    {
                        FormattedAddress = add.Address.FirstOrDefault().FormattedAddress,
                        Longitude = add.Address.FirstOrDefault().Longitude,
                        Latitude= add.Address.FirstOrDefault().Latitude,
                    }).FirstOrDefault();
                    if (address != null)
                    {
                        return Ok(new { address });
                    }
                    else
                    {
                        return NotFound(new { message = "Customer not found" });
                    }
                }
                return Unauthorized(new { message = "Unauthorised" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error" });
            }

        }

        [HttpPost]
        [Route("api/UserManage/address/save")]
        public async Task<IActionResult> SaveAddress([FromBody] SaveAddressModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.IserId);
                if (user != null)
                {
                    var customer = _context.Customers.FirstOrDefault(cus => cus.UserId == user.Id);
                    if (customer != null)
                    {
                        var address = _context.Addresses.FirstOrDefault(add => add.CustomerId == customer.Id);
                        if (address == null)
                        {
                            var newAddress = new Address
                            {
                                CustomerId = customer.Id,
                                FormattedAddress = model.FormattedAddress,
                                Latitude = model.Latitude,
                                Longitude = model.Longitude,
                                CreatedDateTime = DateTime.UtcNow,
                                LastUpdateDateTime = DateTime.UtcNow,
                                IsActive = true,
                                Customer = customer,
                            };

                            _context.Addresses.Add(newAddress);
                        }
                        else // Update existing address
                        {
                            address.FormattedAddress = model.FormattedAddress;
                            address.Latitude = model.Latitude;
                            address.Longitude = model.Longitude;
                            address.LastUpdateDateTime = DateTime.UtcNow;
                        }
                        await _context.SaveChangesAsync();
                        return Ok(new { message = "Address saved successfully" });
                    }
                    else
                    {
                        return NotFound(new { message = "Customer not found" });
                    }
                }
                else
                {
                    return Unauthorized(new { message = "Unauthorised" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error" });
            }
        }

    }
}
