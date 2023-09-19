using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BrightBoostApplication.Data;
using BrightBoostApplication.Models;
using BrightBoostApplication.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace BrightBoostApplication.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class TermsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TermsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Terms
        public async Task<IActionResult> Index()
        {
              return _context.Terms != null ? 
                          View() :
                          Problem("Entity set 'ApplicationDbContext.Terms'  is null.");
        }

        [HttpGet]
        public async Task<JsonResult> GetAllTermsAsync()
        {
            var terms = new List<Term>();
            if (_context.Terms != null)
            {
                terms = await _context.Terms.Where(i => i.isActive == true).ToListAsync();
            }     
            return Json(terms);
        }

        [HttpPost]
        public async Task<JsonResult> Create(string termName, DateTime startDate, DateTime endDate)
        {
            if (!string.IsNullOrEmpty(termName))
            {
                var term = new Term(){
                    TermName = termName,
                    startDate = startDate,
                    endDate = endDate,
                    updateDate= DateTime.Now,
                    createdDate= DateTime.Now,
                    isActive = true
                };
                _context.Add(term);
                await _context.SaveChangesAsync();
                return Json(true);
            }
            return Json(false);
        }

        // GET: Terms/Details/5
        [HttpGet]
        public async Task<JsonResult> Details(int? id)
        {
            if (id == null || _context.Terms == null)
            {
                return Json(false);
            }

            var term = await _context.Terms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (term == null)
            {
                return Json(false);
            }

            return Json(term);
        }

        [HttpPost]
        public async Task<JsonResult> Update([FromBody]TermViewModel updatedTerm)
        {
            if (updatedTerm.Id == null || _context.Terms == null)
            {
                return Json(false);
            }

            var existingTerm = await _context.Terms.FirstOrDefaultAsync(m => m.Id == updatedTerm.Id);

            if (existingTerm != null)
            {
                if (!string.IsNullOrEmpty(updatedTerm.TermName) || updatedTerm.StartDate != null || updatedTerm.EndDate != null)
                {
                    // Update the properties of the existing term with the new values
                    existingTerm.TermName = updatedTerm.TermName;
                    existingTerm.startDate = updatedTerm.StartDate;
                    existingTerm.endDate = updatedTerm.EndDate;
                    existingTerm.updateDate = DateTime.Now;
                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    return Json(true); // Successful update
                }
                return Json(false);
            }
            else
            {
                return Json(false); // Term not found
            }
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteTerm(int? id)
        {
            if (id == null || _context.Terms == null)
            {
                return Json(false);
            }

            var term = await _context.Terms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (term == null)
            {
                return Json(false);
            }
            term.isActive = false;
            await _context.SaveChangesAsync();
            return Json(true);
        }
    }
}
