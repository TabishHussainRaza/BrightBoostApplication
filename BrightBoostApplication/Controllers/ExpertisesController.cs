using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BrightBoostApplication.Data;
using BrightBoostApplication.Models;
using Microsoft.AspNetCore.Identity;
using BrightBoostApplication.Models.ViewModel;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace BrightBoostApplication.Controllers
{
    [Authorize(Roles = "Tutor")]
    public class ExpertisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public ExpertisesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Expertises
        public async Task<IActionResult> Index()
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

        [HttpGet]
        public async Task<JsonResult> GetMyExpertise()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutor = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutor != null)
                {
                    var expertises = _context.Expertise.Where(e => e.TutorId == tutor.Id).Include(s=>s.Subject).Select(tc => new SubjectViewModel
                    {
                        Id = tc.Subject.Id,
                        SubjectName = tc.Subject.subjectName,
                        SubjectDescription = tc.Subject.subjectDescription,
                    }).ToList();
                    return Json(expertises);
                }

            }
            return Json(null);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllSubjects()
        {
            var subjects = new List<Subject>();
            if (_context.Subjects != null)
            {
                subjects = await _context.Subjects.Where(i => i.isActive == true).ToListAsync();
            }
            return Json(subjects);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllAvailability()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutor = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutor != null)
                {
                    var availability = tutor?.Availability?.Split(',');
                    return Json(availability);
                }

            }
            return Json(null);
        }

        [HttpPost]
        public async Task<JsonResult> AddSubjectMapping([FromBody] ExpertiseViewModel data)
        {
            try
            {
                var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.Id == data.TutorId);
                if (tutor == null)
                {
                    return Json(new { status = false, message = "Tutor not found." });
                }

                if (data.RemoveAll)
                {
                    var existingExpertiseCourses = _context.Expertise
                        .Where(tc => tc.TutorId == tutor.Id)
                        .ToList();

                    _context.Expertise.RemoveRange(existingExpertiseCourses);
                }
                else
                {
                    var existingSubjectIds = _context.Expertise
                        .Where(tc => tc.TutorId == tutor.Id)
                        .Select(tc => tc.SubjectId)
                        .ToList();

                    var subjectsToRemove = existingSubjectIds.Except(data.SubjectIds).ToList();

                    if (subjectsToRemove.Any())
                    {
                        var termCoursesToRemove = _context.Expertise
                            .Where(tc => tc.TutorId == tutor.Id && subjectsToRemove.Contains(tc.SubjectId))
                            .ToList();

                        _context.Expertise.RemoveRange(termCoursesToRemove);
                    }

                    var subjectsToAdd = data.SubjectIds.Except(existingSubjectIds).ToList();

                    foreach (var subjectId in subjectsToAdd)
                    {
                        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
                        if (subject != null)
                        {
                            var expertise = new Expertise()
                            {
                                TutorId = tutor.Id,
                                Tutor = tutor,
                                SubjectId = subject.Id,
                                Subject = subject,
                            };
                            _context.Expertise.Add(expertise);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { status = true }); // Successful update
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddExpAvailability(int id, string availability)
        {
            try
            {
                var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.Id == id);
                if (tutor == null)
                {
                    return Json(new { status = false, message = "Tutor not found." });
                }

                tutor.Availability = availability;
                await _context.SaveChangesAsync();
                return Json(new { status = true }); // Successful update
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public IActionResult TimeTable()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAllMySessions()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var tutor = _context.Tutors.Where(s => s.userId == userId).FirstOrDefault();
                if (tutor != null)
                {
                    var allocations = _context.TutorAllocations.Where(t => t.TutorId == tutor.Id).Include(s => s.Session).ThenInclude(o => o.TermCourse).ThenInclude(o => o.Term).Select(tc => new SessionViewModel
                    {
                        SessionName = tc.Session.SessionName,
                        SessionDay = tc.Session.SessionDay,
                        SessionVenue = tc.Session.SessionVenue,
                        Id = tc.Session.Id,
                        TermCourse = tc.Session.TermCourse
                    }).ToList();

                    var sessionsList = new List<SessionViewModel>();
                    allocations.ForEach(sc =>
                    {
                        // Calculate the number of days in the term
                        int daysDifference = (int)(sc.TermCourse.Term.endDate.Value - sc.TermCourse.Term.startDate.Value).TotalDays;



                        // Calculate the initial session date based on the term start date and session day
                        DateTime initialSessionDate = sc.TermCourse.Term.startDate.Value;
                        string sessionDayName = sc.SessionDay; // Session day name as a string

                        // Find the first occurrence of the session day
                        while (initialSessionDate.DayOfWeek.ToString() != sessionDayName)
                        {
                            initialSessionDate = initialSessionDate.AddDays(1);
                        }

                        // Calculate the end date based on the term end date
                        DateTime termEndDate = sc.TermCourse.Term.endDate.Value;
                        DateTime sessionDate = initialSessionDate;

                        while (sessionDate <= termEndDate)
                        {
                            var mySession = new SessionViewModel()
                            {
                                SessionName = sc.TermCourse.Title + " " + sc.SessionName,
                                SessionDay = sc.SessionDay,
                                SessionVenue = sc.SessionVenue,
                                Id = sc.Id,
                                TermCourse = new TermCourse(), // Create a new instance to avoid object cycle
                                startTime = sessionDate // Assign the calculated session date to startTime
                            };

                            sessionsList.Add(mySession);
                            sessionDate = sessionDate.AddDays(7);
                        }
                    });

                    return Json(new { status = false, sessions = sessionsList });
                }
            }
            return Json(new { status = false, message = "Processing Failed." });
        }
    }
}
