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
using SQLitePCL;
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
            return Json(_context.Users.Where(us => us.GroupId.HasValue && us.GroupId == id).ToList());
        }

        public JsonResult GetMyBranch(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(false);
            }

            var staffBranch = _context.Staffs.Where(staff => staff.UserId == userId && staff.IsActive == true).Select(us => us.BranchId).FirstOrDefault();
            return Json(staffBranch);
        }

        [HttpPost]
        public JsonResult SaveBranch(string userId, int branchId, bool remove= false)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(false);
            }

            var branch = _context.Branches.FirstOrDefault(branch => branch.Id == branchId);
            var user = _context.Users.FirstOrDefault(user => user.Id == userId);

            if ((branch == null && !remove) || user == null)
            {
                return Json(false);
            }

            var staffBranch = _context.Staffs.Where(staff => staff.UserId == user.Id && staff.IsActive == true).FirstOrDefault();

            if (staffBranch == null)
            {
                var newBranchAssignment = new Staff()
                {
                    Branch = branch,
                    UserId = user.Id,
                    CreatedDateTime = DateTime.UtcNow,
                    LastUpdateDateTime = DateTime.UtcNow,
                    BranchId = branch.Id,
                    IsActive= true
                };

                _context.Staffs.Add(newBranchAssignment);
                _context.SaveChanges();

                return Json(true);
            }

            if(remove)
            {
                staffBranch.IsActive = false;
            }
            else
            {
                staffBranch.Branch = branch;
                staffBranch.BranchId = branch.Id;
            }
            staffBranch.CreatedDateTime = DateTime.UtcNow;
            staffBranch.LastUpdateDateTime = DateTime.UtcNow;
            _context.Entry(staffBranch).State = EntityState.Modified;
            _context.SaveChanges();
            return Json(true);
        }
    }
}
