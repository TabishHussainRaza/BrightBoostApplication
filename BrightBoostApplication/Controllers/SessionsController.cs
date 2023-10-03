using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BrightBoostApplication.Data;
using BrightBoostApplication.Models;
using BrightBoostApplication.Models.ViewModel;

namespace BrightBoostApplication.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index(int id)
        {
            if (id == null || _context.TermCourses == null)
            {
                return NotFound();
            }

            var termCourse = _context.TermCourses.Where(i => i.Id == id).FirstOrDefault();
            if (termCourse == null)
            {
                return NotFound();
            }

            return View(termCourse);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllSessionsAsync(int id)
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
            var sessions = _context.Sessions.Where(s=>s.fkId == term.Id && s.isActive == true).Select(tc => new SessionViewModel
            {
                SessionName = tc.SessionName,
                SessionDay = tc.SessionDay,
                SessionVenue = tc.SessionVenue,
                Id = tc.Id,
                SignUpCount = _context.StudentSignUps.Where(s => s.SessionId == tc.Id).Count(),
            }).ToList();
            return Json(sessions);
        }

        [HttpPost]
        public async Task<JsonResult> Create(int termCouseId, string SessionName, string SessionDay, string SessionVenue,int? sessionCap, DateTime? startTime, DateTime? endTime)
        {
            if (termCouseId != 0 && !string.IsNullOrEmpty(SessionName) && !string.IsNullOrEmpty(SessionDay) && !string.IsNullOrEmpty(SessionVenue))
            {
                var termCourse = _context.TermCourses.Where(i => i.Id == termCouseId).FirstOrDefault();
                if(termCourse == null)
                {
                    return Json(false);
                }

                var session = new Session()
                {
                    SessionName = SessionName,
                    SessionDay = SessionDay,
                    SessionVenue = SessionVenue,
                    isActive = true,
                    SessionColor = "",
                    createdDate=DateTime.Now,
                    updateDate=DateTime.Now,
                    fkId = termCourse.Id,
                    TermCourse = termCourse,
                    MaxNumber = sessionCap,
                    currentCap =0
                };
                _context.Add(session);
                await _context.SaveChangesAsync();
                return Json(true);
            }
            return Json(false);
        }

        // GET: Sessions/Details/5
        [HttpGet]
        public async Task<JsonResult> Details(int? id)
        {
            if (id == null || _context.Sessions == null)
            {
                return Json(false);
            }

            var session = await _context.Sessions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return Json(false);
            }

            return Json(session);
        }

        [HttpPost]
        public async Task<JsonResult> Update([FromBody] SessionUpdateViewModel updatedSession)
        {
            if (updatedSession.Id == null || _context.Sessions == null)
            {
                return Json(false);
            }

            var existingSession = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == updatedSession.Id);

            if (existingSession != null)
            {
                if (!string.IsNullOrEmpty(updatedSession.SessionName) || !string.IsNullOrEmpty(updatedSession.SessionDay) || string.IsNullOrEmpty(updatedSession.SessionVenue) || updatedSession.startTime != null || updatedSession.EndTime != null)
                {
                    // Update the properties of the existing session with the new values
                    existingSession.SessionName = updatedSession.SessionName;
                    existingSession.SessionDay = updatedSession.SessionDay;
                    existingSession.SessionVenue = updatedSession.SessionVenue;
                    existingSession.updateDate = DateTime.Now;
                    existingSession.MaxNumber = updatedSession.MaxNumber;
                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    return Json(true); // Successful update
                }
                return Json(false);
            }
            else
            {
                return Json(false); // Session not found
            }
        }


        [HttpDelete]
        public async Task<JsonResult> DeleteSession(int? id)
        {
            if (id == null || _context.Sessions == null)
            {
                return Json(false);
            }

            var session = await _context.Sessions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return Json(false);
            }
            session.isActive = false;
            await _context.SaveChangesAsync();
            return Json(true);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllTutors(int id)
        {
            if (id == null)
            {
                return Json(new { status = false, message = $"No Id provided" });
            }

            var session = _context.Sessions.Where(i => i.Id == id).Include(s=>s.TermCourse).FirstOrDefault();
            if (session == null)
            {
                return Json(new { status = false, message = $"No Session Details Found" });
            }

            var AllTutors = _context.Tutors.Include(e=>e.Expertise).ToList();
            var relTutors = AllTutors.Where(t => t.Expertise.Any(e => e.SubjectId == session.TermCourse.SubjectId) && t.isActive == true && t.Availability.Contains(session.SessionDay)).Select(tc => new TutorViewModel
            {
                TutorId = tc.Id,
                Name = _context.Users.Where(u => u.Id == tc.userId).FirstOrDefault().firstName,
            }).ToList();
            return Json(new { status = true, data = relTutors });
        }

        [HttpGet]
        public async Task<JsonResult> GetMyTutors(int id)
        {
            if (id == null)
            {
                return Json(new { status = false, message = $"No Id provided" });
            }

            var session = _context.Sessions.Where(i => i.Id == id).Include(s => s.TutorAllocation).ThenInclude(ta=>ta.Tutor).FirstOrDefault();
            if (session == null)
            {
                return Json(new { status = false, message = $"No Session Details Found" });
            }
            try
            {
                var relTutors = session.TutorAllocation.Select(tc => new TutorViewModel
                {
                    TutorId = tc.Id,
                    Name = _context.Users.Where(u => u.Id == tc.Tutor.userId).FirstOrDefault().firstName,
                }).ToList();
                return Json(new { status = true, data = relTutors });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = $"An error occurred: {ex.Message}" });
            }
            
        }

        [HttpPost]
        public async Task<JsonResult> AllocateTutor([FromBody] TutorCRUDViewModel data)
        {
            try
            {
                var session = await _context.Sessions.FirstOrDefaultAsync(t => t.Id == data.SessionId);
                if (session == null)
                {
                    return Json(new { status = false, message = "Term not found." });
                }

                if (data.RemoveAll)
                {
                    var existing = _context.TutorAllocations
                        .Where(tc => tc.SessionId == session.Id)
                        .ToList();

                    _context.TutorAllocations.RemoveRange(existing);
                }
                else
                {
                    var existingTutorsIds = _context.TutorAllocations
                        .Where(tc => tc.SessionId == session.Id)
                        .Select(tc => tc.TutorId)
                        .ToList();

                    var tutorsToRemove = existingTutorsIds.Except(data.TutorId).ToList();

                    if (existingTutorsIds.Any())
                    {
                        var termCoursesToRemove = _context.TutorAllocations
                            .Where(tc => tc.SessionId == session.Id && existingTutorsIds.Contains(tc.TutorId))
                            .ToList();

                        _context.TutorAllocations.RemoveRange(termCoursesToRemove);
                    }

                    var toAdd = data.TutorId.Except(existingTutorsIds).ToList();

                    foreach (var tutorId in toAdd)
                    {
                        var tutor = await _context.Tutors.FirstOrDefaultAsync(s => s.Id == tutorId);
                        if (tutor != null)
                        {
                            var allTutor = new TutorAllocation()
                            {
                                Tutor = tutor,
                                Session = session,
                                SessionId = session.Id,
                                TutorId = tutorId
                            };
                            _context.TutorAllocations.Add(allTutor);
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
    }
}
