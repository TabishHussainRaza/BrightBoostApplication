using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BrightBoostApplication.Data;
using BrightBoostApplication.Models;
using AutoMapper;
using BrightBoostApplication.Models.ViewModel;

namespace BrightBoostApplication.Controllers
{
    public class TermCoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper? _mapper;

        public TermCoursesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: TermCourses
        public async Task<IActionResult> Index(int id)
        {
            if (id == null || _context.TermCourses == null)
            {
                return NotFound();
            }

            var term = _context.Terms.Where(i => i.Id == id).FirstOrDefault();
            if (term == null )
            {
                return NotFound();
            }
            return View(term);
        }

        public async Task<JsonResult> CurrentTermCourses(int id)
        {
            if (id == null || _context.TermCourses == null)
            {
                return Json(new { status = false, message = $"No Id provided" });
            }

            var term = _context.Terms.Where(i => i.Id == id).FirstOrDefault();
            if (term == null)
            {
                return Json(new { status = false, message = $"No Term Details Found" });
            }
            else
            {

                var termCourse = _context.TermCourses.Where(m => m.TermId == term.Id).Include(tc => tc.Subject).Select(tc => new TermSubjectViewModel
                {
                    TermCourseId = tc.Id,
                    subjectName = tc.Subject.subjectName,
                    subjectDescription = tc.Subject.subjectDescription,
                    createdDate = tc.Subject.createdDate,
                    updateDate = tc.Subject.updateDate,
                    isActive = tc.Subject.isActive,
                }).ToList();
                return Json(termCourse);
                
            }
        }

        // GET: TermCourses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TermCourses == null)
            {
                return NotFound();
            }

            var termCourse = await _context.TermCourses
                .Include(t => t.Subject)
                .Include(t => t.Term)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (termCourse == null)
            {
                return NotFound();
            }

            return View(termCourse);
        }

        // GET: TermCourses/Create
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id");
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Id");
            return View();
        }

        // POST: TermCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectId,TermId,Title")] TermCourse termCourse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(termCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", termCourse.SubjectId);
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Id", termCourse.TermId);
            return View(termCourse);
        }

        // GET: TermCourses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TermCourses == null)
            {
                return NotFound();
            }

            var termCourse = await _context.TermCourses.FindAsync(id);
            if (termCourse == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", termCourse.SubjectId);
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Id", termCourse.TermId);
            return View(termCourse);
        }

        // POST: TermCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectId,TermId,Title")] TermCourse termCourse)
        {
            if (id != termCourse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(termCourse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TermCourseExists(termCourse.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", termCourse.SubjectId);
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Id", termCourse.TermId);
            return View(termCourse);
        }

        // GET: TermCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TermCourses == null)
            {
                return NotFound();
            }

            var termCourse = await _context.TermCourses
                .Include(t => t.Subject)
                .Include(t => t.Term)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (termCourse == null)
            {
                return NotFound();
            }

            return View(termCourse);
        }

        // POST: TermCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TermCourses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TermCourses'  is null.");
            }
            var termCourse = await _context.TermCourses.FindAsync(id);
            if (termCourse != null)
            {
                _context.TermCourses.Remove(termCourse);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TermCourseExists(int id)
        {
          return (_context.TermCourses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
