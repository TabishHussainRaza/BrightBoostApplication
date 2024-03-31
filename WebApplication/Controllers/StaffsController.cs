using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Utility;

namespace WebApplication.Controllers
{
    public class StaffsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataAccess _dataAccess;


        public StaffsController(ApplicationDbContext context, DataAccess dataAccess)
        {
            _context = context;
            _dataAccess = dataAccess;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllUsers(int id)
        {
            /*var userId = this.User.GetId();
            var groupId = _context.Users.Where(spuser => spuser.Id == userId).FirstOrDefault();*/
            return Json(_context.Users.Where(us => us.GroupId.HasValue && us.GroupId == id).ToList());
            /*using (var connection = _dataAccess.GetConnection())
            {
                var user = this.User;
                
                connection.Open();
                var result = connection.Query("SELECT * FROM MyTable");

            }*/

            
        }
    }
}
