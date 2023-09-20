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
//    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            return _context.Sessions != null ?
                        View(await _context.Sessions.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Sessions'  is null.");
        }

        [HttpGet]
        public async Task<JsonResult> GetAllSessionsAsync()
        {
            var sessions = new List<Term>();
            if (_context.Sessions != null)
            {
                sessions = await _context.Terms.Where(i => i.isActive == true).ToListAsync();
            }
            return Json(sessions);
        }

        [HttpPost]
        public async Task<JsonResult> Create(string SessionName, string SessionDay, string SessionVenue, DateTime startTime, DateTime endTime)
        {
            if (!string.IsNullOrEmpty(SessionName))
            {
                var session = new Session()
                {
                    SessionName = SessionName,
                    SessionDay = SessionDay,
                    SessionVenue = SessionVenue,
                    startTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    isActive = true,
                    SessionColor = "red",
                    fkId = -1
                };
                _context.Add(session);
                await _context.SaveChangesAsync();
                return Json(true);
            }
            return Json(false);
        }

        // GET: Sessions/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
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
        public async Task<JsonResult> Update([FromBody] SessionViewModel updatedSession)
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
                    existingSession.startTime = updatedSession.startTime;
                    existingSession.EndTime = updatedSession.EndTime;
                    existingSession.updateDate = DateTime.Now;
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
    }
}
