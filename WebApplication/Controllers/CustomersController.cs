using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.ViewModel;
using WebApplication.Utility;

namespace WebApplication.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dapper;

        public CustomersController(ApplicationDbContext context, DataAccess dapper)
        {
            _context = context;
            _dapper = dapper.GetConnection();
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var allCustomers = _dapper.Query("SELECT * FROM AspNetUsers us JOIN Customers cs ON cs.UserId = us.Id").ToList();
            return Json(allCustomers);
        }


        [HttpGet]
        public JsonResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(false);
            }
            
            var customer = _dapper.Query(@"SELECT us.*, ads.FormattedAddress, ads.Latitude, ads.Longitude 
                                            FROM AspNetUsers us 
                                            JOIN Customers cs ON cs.UserId = us.Id 
                                            LEFT JOIN Addresses ads ON ads.CustomerId = cs.Id 
                                            WHERE us.Id = @userId", new { userId  = id }).FirstOrDefault();
            return Json(customer);
        }
    }
}
