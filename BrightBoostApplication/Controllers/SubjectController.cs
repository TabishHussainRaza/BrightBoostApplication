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
    public class SubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
              return _context.Subjects != null ?
                          View() :
                          Problem("Entity set 'ApplicationDbContext.Subjects'  is null.");
        }

        [HttpGet]
        public async Task<JsonResult> GetAllSubjects()
        {
            var subjects = new List<Subject>();
            if (_context.Terms != null)
            {
                subjects = await _context.Subjects.Where(i => i.isActive == true).ToListAsync();
            }
            return Json(subjects);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        public async Task<JsonResult> CreateSubject(string name, string description)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(description))
            {
                var subject = new Subject()
                {
                    subjectName = name,
                    subjectDescription = description,
                    updateDate = DateTime.Now,
                    createdDate = DateTime.Now,
                    isActive = true
                };
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return Json(true);
            }
            return Json(false);
        }

        // GET: Subject/Details/5
        [HttpGet]
        public async Task<JsonResult> Details(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return Json(false);
            }

            var Subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Subject == null)
            {
                return Json(false);
            }

            return Json(Subject);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateSubject([FromBody] SubjectViewModel SubjectViewModel)
        {
            if (!string.IsNullOrEmpty(SubjectViewModel.SubjectName) && !string.IsNullOrEmpty(SubjectViewModel.SubjectDescription))
            {
                var existingItem= await _context.Subjects.FirstOrDefaultAsync(m => m.Id == SubjectViewModel.Id);

                if (existingItem != null)
                {
                    // Update the properties of the existing item with the new values
                    existingItem.subjectName = SubjectViewModel.SubjectName;
                    existingItem.subjectDescription = SubjectViewModel.SubjectDescription;
                    existingItem.updateDate = DateTime.Now;
                    // Save changes to the database
                    await _context.SaveChangesAsync();
                    return Json(true); // Successful update
                }
                else
                {
                    return Json(false); // Not found
                }
            }
            return Json(false);
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteSubject(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return Json(false);
            }

            var Subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Subject == null)
            {
                return Json(false);
            }
            Subject.isActive = false;
            await _context.SaveChangesAsync();
            return Json(true);
        }
    }
}