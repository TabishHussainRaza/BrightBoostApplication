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
    public class CategorySubTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper? _mapper;

        public CategorySubTypesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index(int? id)
        {
            if (id == null || _context.CategoryTypes == null)
            {
                return Redirect("Home/Error");
            }
            ViewBag.typeId = _context.CategoryTypes.Where(ret => ret.Id == id).Select(re => re.Id).FirstOrDefault();
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll(int? id)
        {
            var subTypes = new List<CategorySubTypesVM>();
            if (_context.CategoryTypes != null && _context.CategorySubTypes != null)
            {
                var type = _context.CategoryTypes.Where(ret => ret.Id == id).FirstOrDefault();

                if (type == null)
                {
                    return Json(subTypes);
                }

                subTypes = await _context.CategorySubTypes.Where(i => i.IsActive == true && i.ProductTypeId == type.Id).Include(tc => tc.ProductType).Select(subType => new CategorySubTypesVM()
                {
                    SubTypeName = subType.SubTypeName,
                    IsActive = subType.IsActive,
                    LastUpdateDateTime = subType.LastUpdateDateTime,
                    CreatedDateTime = subType.CreatedDateTime,
                    ProductTypeId = subType.ProductTypeId,
                    Id = subType.Id

                }).ToListAsync();
            }
            return Json(subTypes);
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] CategorySubTypesVM Model)
        {
            var storingState = 0;

            if (Model?.Id != 0)
            {
                var existing = await _context.CategorySubTypes.Where(i => i.Id == Model.Id).FirstOrDefaultAsync();

                if (existing == null)
                {
                    return Json(false);
                }
                existing.SubTypeName = Model.SubTypeName;
                existing.LastUpdateDateTime = DateTime.UtcNow;

                _context.Entry(existing).State = EntityState.Modified;
                storingState = await _context.SaveChangesAsync();
                return Json(storingState);
            }

            var mod = _mapper.Map<CategorySubType>(Model);
            mod.IsActive = true;
            mod.CreatedDateTime = DateTime.UtcNow;
            mod.LastUpdateDateTime = DateTime.UtcNow;
            _context.CategorySubTypes.Add(mod);
            storingState = await _context.SaveChangesAsync();
            return Json(storingState);
        }

        [HttpGet]
        public async Task<JsonResult> Details(int? id)
        {
            if (id == null || _context.CategorySubTypes == null)
            {
                return Json(false);
            }

            var Group = await _context.CategorySubTypes
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
            if (id == null || _context.CategorySubTypes == null)
            {
                return Json(false);
            }

            var Group = await _context.CategorySubTypes
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
