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
                         .Where(tc => tc.TutorId == tutor.Id).Include(tc => tc.Session)
                         .ThenInclude(u=>u.TermCourse).ThenInclude(p=>p.Subject)
                         .GroupBy(g => new { g.Session.TermCourse.Id, g.Session.TermCourse.Title })
                         .Select(tc => new TermSubjectViewModel
                         {
                             TermCourseId = tc.Key.Id,
                             subjectName = tc.Key.Title
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
                       Id = tc.Session.Id,
                       SignUpCount = _context.StudentSignUps.Where(s => s.SessionId == tc.Session.Id).Count(),
                    }).ToList();
                    return Json(allocations);
                }

            }
            return Json(false);
        }
        public async Task<IActionResult> CurrentSessions(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutor = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutor != null)
                {
                    var allocations = _context.TutorAllocations.Where(t => t.TutorId == tutor.Id && t.Session.Id == id).Include(s => s.Session).ThenInclude(o => o.TermCourse).Select(tc => new SessionViewModel
                    {
                        SessionName = tc.Session.SessionName,
                        SessionDay = tc.Session.SessionDay,
                        SessionVenue = tc.Session.SessionVenue,
                        Id = tc.Session.Id
                    }).FirstOrDefault();
                    return View(allocations);
                }

            }
            return Unauthorized();
        }

        public async Task<JsonResult> GetSessions(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutor = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutor != null)
                {
                    var allocations = _context.TutorAllocations.Where(t => t.TutorId == tutor.Id && t.Session.Id == id).Include(s => s.Session).ThenInclude(o => o.TermCourse).ThenInclude(o => o.Term).Select(tc => new SessionViewModel
                    {
                        SessionName = tc.Session.SessionName,
                        SessionDay = tc.Session.SessionDay,
                        SessionVenue = tc.Session.SessionVenue,
                        Id = tc.Session.Id,
                        TermCourse = tc.Session.TermCourse
                    }).FirstOrDefault();

                    if (allocations != null)
                    {
                        // Calculate the number of days in the term
                        int daysDifference = (int)(allocations.TermCourse.Term.endDate.Value - allocations.TermCourse.Term.startDate.Value).TotalDays;

                        var sessionsList = new List<SessionViewModel>();

                        // Calculate the initial session date based on the term start date and session day
                        DateTime initialSessionDate = allocations.TermCourse.Term.startDate.Value;
                        string sessionDayName = allocations.SessionDay; // Session day name as a string

                        // Find the first occurrence of the session day
                        while (initialSessionDate.DayOfWeek.ToString() != sessionDayName)
                        {
                            initialSessionDate = initialSessionDate.AddDays(1);
                        }

                        // Calculate the end date based on the term end date
                        DateTime termEndDate = allocations.TermCourse.Term.endDate.Value;
                        DateTime sessionDate = initialSessionDate;

                        while (sessionDate <= termEndDate)
                        {
                            var mySession = new SessionViewModel()
                            {
                                SessionName = allocations.SessionName,
                                SessionDay = allocations.SessionDay,
                                SessionVenue = allocations.SessionVenue,
                                Id = allocations.Id,
                                TermCourse = new TermCourse(),
                                startTime = sessionDate, // Assign the calculated session date to startTime
                            };

                            sessionsList.Add(mySession);
                            sessionDate = sessionDate.AddDays(7);
                        }

                        return Json(sessionsList);
                    }
                }

            }
            return Json(false);
        }
    }
}
