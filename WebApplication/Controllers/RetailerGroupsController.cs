using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.ViewModel;

namespace WebApplication.Controllers
{
    public class RetailerGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper? _mapper;

        public RetailerGroupsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: RetailerGroups
        public async Task<IActionResult> Index()
        {
              return View(await _context.Branches.ToListAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetAllRetailer()
        {
            var retailer = new List<Branch>();
            if (_context.Branches != null)
            {
                retailer = await _context.Branches.Where(i => i.IsActive == true).ToListAsync();
            }
            return Json(retailer);
        }

        [HttpPost]
        public async Task<JsonResult> CreateRetailer([FromBody] BranchVM GroupModel)
        {
            var storingState = 0;

            if (GroupModel?.Id != 0) { 
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
                existing.CreatedDateTime = DateTime.UtcNow;
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
        public async Task<JsonResult> DeleteRetailer(int? id)
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
