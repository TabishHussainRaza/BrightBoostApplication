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
using Microsoft.AspNetCore.Identity;
using ServiceStack;
using AutoMapper;
using BrightBoostApplication.Data.Migrations;

namespace BrightBoostApplication.Controllers
{
    [Authorize(Roles ="Tutor")]
    public class TutorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IMapper? _mapper;

        public TutorsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutor = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutor != null)
                {
                    return View(tutor);
                }

            }

            // User is not authenticated or not found
            return Unauthorized();

        }

        public async Task<JsonResult> MyAllocation()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutor = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutor != null)
                {
                    var existingSubjectIds = _context.TutorAllocations
                         .Where(tc => tc.TutorId == tutor.Id).Include(tc => tc.Session).ThenInclude(u=>u.TermCourse).ThenInclude(p=>p.Subject).Select(tc => new TermSubjectViewModel
                         {
                             TermCourseId = tc.Session.TermCourse.Id,
                             subjectName = tc.Session.TermCourse.Title
                         }).ToList();
                    return Json(existingSubjectIds);
                }

            }
            return Json(null);

        }

        public async Task<IActionResult> Course(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutors = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutors != null)
                {
                    var termCourse = _context.TermCourses.Where(i => i.Id == id).Include(tc => tc.Subject).Select(tc => new TermSubjectViewModel
                    {
                        TermCourseId = tc.Id,
                        subjectName = tc.Title,
                        subjectDescription = tc.Subject.subjectDescription,
                        createdDate = tc.Subject.createdDate,
                        updateDate = tc.Subject.updateDate,
                        isActive = tc.Subject.isActive,
                        TutorId = tutors.Id
                    }).FirstOrDefault();
                    return View(termCourse);
                }

            }
            return Unauthorized();
        }

        public async Task<JsonResult> GetAllocatedSessions(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutor = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutor != null)
                {
                    var allocations = _context.TutorAllocations.Where(t=>t.TutorId == tutor.Id && t.Session.TermCourse.Id == id).Include(s=>s.Session).ThenInclude(o=>o.TermCourse).Select(tc => new SessionViewModel
                    {
                       SessionName = tc.Session.SessionName,
                       SessionDay = tc.Session.SessionDay,
                       SessionVenue = tc.Session.SessionVenue,
                       Id = tc.Session.Id
                    }).ToList();
                    return Json(allocations);
                }

            }
            return Json(false);
        }
    }
}
