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
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BrightBoostApplication.Controllers
{
    [Authorize(Roles ="Student")]
    public class StudentsTermController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IMapper? _mapper;

        public StudentsTermController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

            // GET: Terms
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var student = _context.Student.Where(s=>s.userId == userId).FirstOrDefault();
                if(student != null)
                {
                    return View(student);
                }
               
            }

            // User is not authenticated or not found
            return Unauthorized();
           
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                if (student != null)
                {
                    return View(student);
                }

            }

            // User is not authenticated or not found
            return Unauthorized();

        }

        [HttpGet]
        public async Task<JsonResult> GetAllCurrentCourse()
        {
            var terms = _context.Terms.Where(t => t.status == (int)Enums.Enums.TermStatus.current).ToList();
            var subjects = new List<TermSubjectViewModel>();
            if(terms.Any())
            {
                foreach(var term in  terms) {
                    var termCourses = _context.TermCourses.Where(i => i.TermId == term.Id).Include(tc => tc.Subject).Select(tc => new TermSubjectViewModel
                    {
                        TermCourseId = tc.Id,
                        subjectName = tc.Title,
                        subjectDescription = tc.Subject.subjectDescription,
                        createdDate = tc.Subject.createdDate,
                        updateDate = tc.Subject.updateDate,
                        isActive = tc.Subject.isActive,
                    }).ToList();
                    subjects.AddRange(termCourses);
                }
            }
            return Json(subjects);
        }

        public async Task<JsonResult> MyRegistrations(int id)
        {
            var existingSubjectIds = _context.StudentCourseSignUps
                         .Where(tc => tc.StudentId == id).Include(tc => tc.TermCourse).Select(tc => new TermSubjectViewModel
                         {
                             TermCourseId = tc.TermCourse.Id,
                             subjectName = tc.TermCourse.Title
                         }).ToList();
            return Json(existingSubjectIds);
        }

        [HttpPost]
        public async Task<JsonResult> AddSubjectMapping([FromBody] TermCourseSignUpViewModel data)
        {
            try
            {
                var student = await _context.Student.FirstOrDefaultAsync(t => t.Id == data.StudentId);
                if (student == null)
                {
                    return Json(new { status = false, message = "Student not found." });
                }

                if (data.RemoveAll)
                {
                    var existingTermCourses = _context.StudentCourseSignUps
                        .Where(tc => tc.StudentId == student.Id)
                        .ToList();

                    _context.StudentCourseSignUps.RemoveRange(existingTermCourses);
                }
                else
                {
                    var existingSubjectIds = _context.StudentCourseSignUps
                         .Where(tc => tc.StudentId == student.Id)
                        .Select(tc => tc.termCourseId)
                        .ToList();

                    var subjectsToRemove = existingSubjectIds.Except(data.SubjectIds).ToList();

                    if (subjectsToRemove.Any())
                    {
                        var termCoursesToRemove = _context.StudentCourseSignUps
                            .Where(tc => tc.StudentId == student.Id && subjectsToRemove.Contains(tc.termCourseId))
                            .ToList();

                        _context.StudentCourseSignUps.RemoveRange(termCoursesToRemove);
                    }

                    var subjectsToAdd = data.SubjectIds.Except(existingSubjectIds).ToList();

                    foreach (var subjectId in subjectsToAdd)
                    {
                        var subject = await _context.TermCourses.FirstOrDefaultAsync(s => s.Id == subjectId);
                        if (subject != null)
                        {
                            var StudentCourseSignUp = new StudentCourseSignUp()
                            {
                                Student = student,
                                termCourseId = subjectId,
                                TermCourse = subject,
                                StudentId = student.Id,
                                createDate = DateTime.Now,
                            };
                            _context.StudentCourseSignUps.Add(StudentCourseSignUp);
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

        public async Task<IActionResult> Course(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                if (student != null)
                {
                    var termCourse = _context.TermCourses.Where(i => i.Id == id).Include(tc => tc.Subject).Select(tc => new TermSubjectViewModel
                    {
                        TermCourseId = tc.Id,
                        subjectName = tc.Title,
                        subjectDescription = tc.Subject.subjectDescription,
                        createdDate = tc.Subject.createdDate,
                        updateDate = tc.Subject.updateDate,
                        isActive = tc.Subject.isActive,
                        StuId = student.Id
                    }).FirstOrDefault();
                    return View(termCourse);
                }

            }
            return Unauthorized();
        }

        public async Task<IActionResult> CourseSession(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                if (student != null)
                {
                    var term = _context.TermCourses.Where(i => i.Id == id).FirstOrDefault();
                    if (term == null)
                    {
                        return Json(new { status = false, message = $"No Term Details Found" });
                    }
                    return View("Sessions", term);
                }

            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<JsonResult> RegisterSession(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Get the user's ID
                    var userId = user.Id;
                    var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                    if (student != null)
                    {
                        var session = _context.Sessions.Where(s=>s.Id == id).Include(u=>u.TermCourse).FirstOrDefault();
                        if (session != null)
                        {
                            var existingSession = _context.StudentSignUps.Where(s => s.StudentId == student.Id && s.Session.TermCourse.Id == session.TermCourse.Id).FirstOrDefault();

                            if (existingSession != null)
                            {
                                _context.StudentSignUps.Remove(existingSession); // Remove the old session sign-up
                                session.currentCap--;
                            }
                            var studentsign = new StudentSignUp()
                            {
                                SessionId = session.Id,
                                Session = session,
                                Student = student,
                                StudentId = student.Id,
                            };
                            session.currentCap++;
                            _context.Add(studentsign);
                            _context.SaveChanges();
                            return Json(new { status = true }); // Successful update
                        }      
                    }
                }
                return Json(new { status = false, message = "Processing Failed." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllSessionsAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                if (student != null)
                {
                    if (id == null || _context.TermCourses == null)
                    {
                        return Json(new { status = false, message = $"No Id provided" });
                    }

                    var term = _context.TermCourses.Where(i => i.Id == id).FirstOrDefault();
                    if (term == null)
                    {
                        return Json(new { status = false, message = $"No Term Details Found" });
                    }
                    var sessions = _context.Sessions.Where(s => s.fkId == term.Id && s.isActive == true).ToList();
                    var sessionsMapped = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
                    var existingSession = _context.StudentSignUps.Where(s => s.StudentId == student.Id && s.Session.TermCourse.Id == term.Id).FirstOrDefault();

                    return Json(new { Sessions = sessionsMapped, RegisteredSessionId = existingSession?.Session?.Id});

                }

            }
            return Json(new { status = false, message = "Processing Failed." });
        }

        public async Task<JsonResult> GetAllocatedSessions(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                if (student != null)
                {
                    var allocations = _context.StudentSignUps.Where(t => t.StudentId == student.Id && t.Session.TermCourse.Id == id).Include(s => s.Session).ThenInclude(o => o.TermCourse).Select(tc => new SessionViewModel
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

        public async Task<IActionResult> CurrentSessions(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Get the user's ID
                var userId = user.Id;
                var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                if (student != null)
                {
                    var allocations = _context.StudentSignUps.Where(t => t.StudentId == student.Id && t.Session.Id == id).Include(s => s.Session).ThenInclude(o => o.TermCourse).Select(tc => new SessionViewModel
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
                var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                if (student != null)
                {
                    var allocations = _context.StudentSignUps.Where(t => t.StudentId == student.Id && t.Session.Id == id).Include(s => s.Session).ThenInclude(o => o.TermCourse).ThenInclude(o => o.Term).Select(tc => new SessionViewModel
                    {
                        SessionName = tc.Session.SessionName,
                        SessionDay = tc.Session.SessionDay,
                        SessionVenue = tc.Session.SessionVenue,
                        Id = tc.Session.Id,
                        TermCourse= tc.Session.TermCourse
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
                                TermCourse = new TermCourse(), // Create a new instance to avoid object cycle
                                startTime = sessionDate // Assign the calculated session date to startTime
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
                var student = _context.Student.Where(s => s.userId == userId).FirstOrDefault();
                if (student != null)
                {
                    var allocations = _context.StudentSignUps.Where(t => t.StudentId == student.Id).Include(s => s.Session).ThenInclude(o => o.TermCourse).ThenInclude(o => o.Term).Select(tc => new SessionViewModel
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
