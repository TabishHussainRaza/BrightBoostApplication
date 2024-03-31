using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RetailersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper? _mapper;

        public RetailersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: 
        public async Task<IActionResult> Index()
        {
            return View(await _context.Retailers.ToListAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var retailers = new List<Retailer>();
            if (_context.Branches != null)
            {
                retailers = await _context.Retailers.Where(i => i.IsActive == true).ToListAsync();
            }
            return Json(retailers);
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] RetailerVM Model)
        {
            var storingState = 0;

            if (Model?.Id != 0)
            {
                var existing = await _context.Retailers.Where(i => i.Id == Model.Id).FirstOrDefaultAsync();

                if (existing == null)
                {
                    return Json(false);
                }
                existing.Name = Model.Name;
                existing.CreatedDateTime = DateTime.UtcNow;
                existing.LastUpdateDateTime = DateTime.UtcNow;

                _context.Entry(existing).State = EntityState.Modified;
                storingState = await _context.SaveChangesAsync();
                return Json(storingState);
            }

            var group = _mapper.Map<Retailer>(Model);
            group.IsActive = true;
            group.CreatedDateTime = DateTime.UtcNow;
            group.LastUpdateDateTime = DateTime.UtcNow;
            _context.Retailers.Add(group);
            storingState = await _context.SaveChangesAsync();
            return Json(storingState);
        }

        [HttpGet]
        public async Task<JsonResult> Details(int? id)
        {
            if (id == null || _context.Retailers == null)
            {
                return Json(false);
            }

            var Group = await _context.Retailers
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
            if (id == null || _context.Retailers == null)
            {
                return Json(false);
            }

            var Group = await _context.Retailers
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
