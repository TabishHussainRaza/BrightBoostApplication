using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Migrations;
using WebApplication.Models;
using WebApplication.Models.ViewModel;
using WebApplication.Utility;

namespace WebApplication.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dataAccess;


        public InventoriesController(ApplicationDbContext context, DataAccess dataAccess)
        {
            _context = context;
            _dataAccess = dataAccess.GetConnection();
        }

        // GET: Inventories
        public IActionResult Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var groupId = _context.Users.Where(user => user.Id == userId).Select(us => us.GroupId).FirstOrDefault();
            var branchId = _context.Staffs.Where(staff => staff.UserId == userId).Select(us => us.BranchId).FirstOrDefault();

            if(branchId== null || branchId <= 0)
            {
                return Redirect("Home/Error");
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetAll(int? id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var groupId = _context.Users.Where(user => user.Id == userId).Select(us => us.GroupId).FirstOrDefault();
            var branchId = _context.Staffs.Where(staff => staff.UserId == userId).Select(us => us.BranchId).FirstOrDefault();

            if (branchId == null || branchId <= 0)
            {
                return Json(new List<dynamic>());
            }

            var products = _dataAccess.Query(@"SELECT pr.*, inv.Quantity, inv.Id AS InventoryId
                                                FROM Products pr 
                                                LEFT JOIN Inventory inv ON pr.Id = inv.ProductId 
                                                WHERE pr.GroupId = @groupId 
                                                AND (BranchId IS NULL OR BranchId = @branchId)", new { groupId = groupId, branchId = branchId }).ToList();

            return Json(products);
        }

        // GET: Inventories/Details/5
        public JsonResult Details(int? id)
        {
            var product = _dataAccess.Query<UpdateVM>(@"SELECT pr.Id AS ProductId, inv.Quantity  AS Quantity, inv.Id
                                                FROM Products pr 
                                                LEFT JOIN Inventory inv ON pr.Id = inv.ProductId 
                                                WHERE pr.Id = @Id", new { Id = id }).FirstOrDefault();

            return Json(product);
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] UpdateVM Model)
        {
            var storingState = 0;

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var groupId = _context.Users.Where(user => user.Id == userId).Select(us => us.GroupId).FirstOrDefault();
            var branch = _context.Staffs.Where(staff => staff.UserId == userId).Select(us => us.Branch).FirstOrDefault();


            if (Model?.Id != 0)
            {
                var existing = await _context.Inventory.Where(i => i.Id == Model.Id).FirstOrDefaultAsync();

                if (existing == null)
                {
                    return Json(false);
                }
                existing.Quantity = Model.Quantity;
                existing.CreatedDateTime = DateTime.UtcNow;
                existing.LastUpdateDateTime = DateTime.UtcNow;

                _context.Entry(existing).State = EntityState.Modified;
                storingState = await _context.SaveChangesAsync();
                return Json(storingState);
            }

            var product = _context.Products.FirstOrDefault(pro => pro.Id == Model.ProductId);

            if (product == null)
            {
                return Json(false);
            }
            var inventory = new Inventory()
            {
                IsActive = true,
                CreatedDateTime = DateTime.UtcNow,
                LastUpdateDateTime = DateTime.UtcNow,
                ProductId = product.Id,
                Product = product,
                Quantity = Model.Quantity,
                Branch = branch,
                BranchId = branch.Id
            };
            
            _context.Inventory.Add(inventory);
            storingState = await _context.SaveChangesAsync();
            return Json(storingState);
        }

    }
}
