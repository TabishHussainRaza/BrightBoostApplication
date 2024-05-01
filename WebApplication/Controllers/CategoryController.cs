using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/Category/GetCategories")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.CategoryTypes
                                .Where(i => i.IsActive == true)
                                .Select(cat => new
                                {
                                    Id = cat.Id,
                                    CategoryName = cat.TypeName,
                                    SubCategories = cat.CategorySubTypes
                                                       .Select(subCat => new
                                                       {
                                                           Id = subCat.Id,
                                                           SubCategoryName = subCat.SubTypeName
                                                       }).ToList()
                                }).ToListAsync();
            return Ok(categories);
        }
    }
}
