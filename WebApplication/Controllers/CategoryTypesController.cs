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
    public class CategoryTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper? _mapper;

        public CategoryTypesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: CategoryTypes
        public IActionResult Index()
        {
              return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var categoryTypes = new List<CategoryType>();
            if (_context.CategoryTypes != null)
            {
                categoryTypes = await _context.CategoryTypes.Where(i => i.IsActive == true).ToListAsync();
            }
            return Json(categoryTypes);
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] CategoryTypeVM Model)
        {
            var storingState = 0;

            if (Model?.Id != 0)
            {
                var existing = await _context.CategoryTypes.Where(i => i.Id == Model.Id).FirstOrDefaultAsync();

                if (existing == null)
                {
                    return Json(false);
                }

                existing.TypeName = Model.TypeName;
                existing.CreatedDateTime = DateTime.UtcNow;
                existing.LastUpdateDateTime = DateTime.UtcNow;

                _context.Entry(existing).State = EntityState.Modified;
                storingState = await _context.SaveChangesAsync();
                return Json(storingState);
            }

            var type = _mapper.Map<CategoryType>(Model);
            type.IsActive = true;
            type.CreatedDateTime = DateTime.UtcNow;
            type.LastUpdateDateTime = DateTime.UtcNow;
            _context.CategoryTypes.Add(type);
            storingState = await _context.SaveChangesAsync();
            return Json(storingState);
        }

        [HttpGet]
        public async Task<JsonResult> Details(int? id)
        {
            if (id == null || _context.CategoryTypes == null)
            {
                return Json(false);
            }

            var Group = await _context.CategoryTypes
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
            if (id == null || _context.CategoryTypes == null)
            {
                return Json(false);
            }

            var Group = await _context.CategoryTypes
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
