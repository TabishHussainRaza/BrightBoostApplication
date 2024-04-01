using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.ViewModel;
using Document = WebApplication.Models.Document;

namespace WebApplication.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        private IMapper? _mapper;

        public ProductsController(IWebHostEnvironment hostingEnvironment, ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var groupId = _context.Users.Where(user => user.Id == userId).Select(us=>us.GroupId).FirstOrDefault();

            var Products = new List<Product>();
            if (_context.Products != null)
            {
                Products = await _context.Products.Where(i => i.IsActive == true && i.GroupId == groupId).ToListAsync();
            }
            return Json(Products);
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] ProductVM Model)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var groupId = _context.Users.Where(user => user.Id == userId).Select(us => us.GroupId).FirstOrDefault();

            var storingState = 0;

            var catSub = await _context.CategorySubTypes.FirstOrDefaultAsync(i => i.Id == Model.CategorySubTypeId);

            if (catSub == null)
            {
                return Json(false);
            }

            if (Model?.Id != 0)
            {
                var existing = await _context.Products.Where(i => i.Id == Model.Id).FirstOrDefaultAsync();

                if (existing == null)
                {
                    return Json(false);
                }

                
                existing.Name = Model.Name;
                existing.Price = Model.Price;
                existing.LongDescription = Model.LongDescription;
                existing.ShortDescription = Model.ShortDescription;
                existing.CategorySubType = catSub;
                existing.CategorySubTypeId = catSub.Id;

                _context.Entry(existing).State = EntityState.Modified;
                storingState = await _context.SaveChangesAsync();
                return Json(existing.Id);
            }

            var prod = _mapper.Map<Product>(Model);
            prod.IsActive = true;
            prod.CategorySubType = catSub;
            prod.CategorySubTypeId = catSub.Id;
            prod.GroupId = groupId;
            _context.Products.Add(prod);
            storingState = await _context.SaveChangesAsync();

            return Json(prod.Id);
        }

        [HttpPost]
        public JsonResult Upload(IEnumerable<IFormFile> files, int productId)
        {
            var product = _context.Products.FirstOrDefault(pr=>pr.Id == productId);

            if (product == null)
            {
                Json(false);
            }

            foreach (var file in files)
            {
                var uniqueFileName = GetUniqueFileName(file.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var document = new Document
                {
                    FileName = uniqueFileName,
                    DocumentPath = filePath,
                    ProductId = productId,
                    Product = product,
                };

                _context.Add(document);
                _context.SaveChanges();
            }

            return Json(true);
        }
       
        private string GetUniqueFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string uniqueName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
            return uniqueName;
        }


        [HttpGet]
        public async Task<JsonResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return Json(false);
            }

            var Product = await _context.Products
                .Where(m => m.Id == id).Include(pr => pr.CategorySubType).FirstOrDefaultAsync();

            if (Product == null)
            {
                return Json(false);
            }

            var prod = _mapper.Map<ProductVM>(Product);
            prod.category = Product.CategorySubType.ProductTypeId;
            prod.ImagePaths = _context.Document.Where(doc =>doc.ProductId ==  prod.Id).Select(
                Doc => new Document()
                {
                    FileName = Doc.FileName,
                    DocumentPath = Doc.DocumentPath,
                    Id = Doc.Id
                }).ToList();
            return Json(prod);
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return Json(false);
            }

            var Product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Product == null)
            {
                return Json(false);
            }
            Product.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(true);
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            var document = _context.Document.FirstOrDefault(m => m.Id == id);

            if (document == null)
            {
                return NotFound();
            }
            if (System.IO.File.Exists(document.DocumentPath))
            {
                var fileContent = System.IO.File.ReadAllBytes(document.DocumentPath);
                return File(fileContent, "application/octet-stream", document.FileName);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteDocument(int? id)
        {
            if (id == null || _context.Document == null)
            {
                return Json(false);
            }

            var doc = await _context.Document
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doc == null)
            {
                return Json(false);
            }
            _context.Remove(doc);
            _context.SaveChanges();
            return Json(true);
        }
    }
}
