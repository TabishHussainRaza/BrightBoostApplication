using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.ViewModel;

namespace WebApplication.Controllers
{
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper? _mapper;

        public BranchesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Branches
        public async Task<IActionResult> Index(int? id)
        {
            if (_context.Retailers == null)
            {
                return Redirect("Home/Error");
            }

            if(id.HasValue)
            {
                ViewBag.retailerId = _context.Retailers.Where(ret => ret.Id == id).Select(re => re.Id).FirstOrDefault();
            }
            else
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var groupId = await _context.Users.Where(user => user.Id == userId).Select(us => us.GroupId).FirstOrDefaultAsync();
                ViewBag.retailerId = groupId;
            }
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll(int? id)
        {
            var branches = new List<BranchVM>();
            if (_context.Branches != null && _context.Retailers != null)
            {
                var retailer = _context.Retailers.Where(ret => ret.Id == id).FirstOrDefault();

                if (retailer == null)
                {
                    return Json(branches);
                }

                branches = await _context.Branches.Where(i => i.IsActive == true && i.RetailerId == retailer.Id).Include(tc => tc.Retailer).Select(GroupModel => new BranchVM() {
                    GroupName = GroupModel.GroupName,
                    FormattedAddress = GroupModel.FormattedAddress,
                    Latitude = GroupModel.Latitude,
                    Longitude = GroupModel.Longitude,
                    ContactName = GroupModel.ContactName,
                    ContactEmail = GroupModel.ContactEmail,
                    ContactPhone = GroupModel.ContactPhone,
                    RetailerId = GroupModel.RetailerId,
                    Id = GroupModel.Id
                }).ToListAsync();
            }
            return Json(branches);
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] BranchVM GroupModel)
        {
            var storingState = 0;

            if (GroupModel?.Id != 0)
            {
                var existing = await _context.Branches.Where(i => i.Id == GroupModel.Id).FirstOrDefaultAsync();

                if (existing == null)
                {
                    return Json(false);
                }
                existing.GroupName = GroupModel.GroupName;
                existing.FormattedAddress = GroupModel.FormattedAddress;
                existing.Latitude = GroupModel.Latitude;
                existing.Longitude = GroupModel.Longitude;
                existing.ContactName = GroupModel.ContactName;
                existing.ContactEmail = GroupModel.ContactEmail;
                existing.ContactPhone = GroupModel.ContactPhone;
                existing.LastUpdateDateTime = DateTime.UtcNow;

                _context.Entry(existing).State = EntityState.Modified;
                storingState = await _context.SaveChangesAsync();
                return Json(storingState);
            }

            var group = _mapper.Map<Branch>(GroupModel);
            group.IsActive = true;
            group.CreatedDateTime = DateTime.UtcNow;
            group.LastUpdateDateTime = DateTime.UtcNow;
            _context.Branches.Add(group);
            storingState = await _context.SaveChangesAsync();
            return Json(storingState);
        }

        [HttpGet]
        public async Task<JsonResult> Details(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return Json(false);
            }

            var Group = await _context.Branches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Group == null)
            {
                return Json(false);
            }

            return Json(Group);
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return Json(false);
            }

            var Group = await _context.Branches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Group == null)
            {
                return Json(false);
            }
            Group.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(true);
        }
    }
}
