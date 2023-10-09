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
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Server;

namespace BrightBoostApplication.Controllers
{
    [Authorize(Roles ="Student")]
    public class StudentQuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentQuestionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Questions
        public async Task<IActionResult> Index(int SessionId, DateTime? SessionDateTime)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Unauthorized();
            }
            var student = _context.Student.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (student == null)
            {
                return Unauthorized();
            }
            var studentSignUp = _context.StudentSignUps.Where(s => s.SessionId == SessionId && s.StudentId == student.Id).FirstOrDefault();
            if (studentSignUp == null)
            {
                return Unauthorized();
            }
            ViewBag.SessionDateTime = SessionDateTime;
            ViewBag.SessionId = SessionId;
            ViewBag.StudentSignUpId= studentSignUp.Id;
            ViewBag.canShowAdd = SessionDateTime.HasValue && SessionDateTime.Value.Date < DateTime.Now.Date ? 0 : 1;
            return _context.Questions != null ? 
                View() :
                Problem("Entity set 'ApplicationDbContext.Questions' is null.");
        }
        
        [HttpGet("StudentQuestions/GetAllMyQuestions")]
        public async Task<JsonResult> GetAllMyQuestionsAsync(int SessionId, string SessionDateTime)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            var student = _context.Student.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (student == null)
            {
                return Json(new { success = false, message = "Student not found." });
            }

            DateTime.TryParseExact(SessionDateTime, "d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

            var session = _context.Sessions.Where(s=>s.Id == SessionId).FirstOrDefault();

            var myQuestions = _context.Questions
                .Where(x => x.StudentSignUp.SessionId == session.Id && x.sessionDate == parsedDate).Include(q=>q.StudentSignUp)
                .Select(
                    tmp => new
                    {
                        tmp.id,
                        tmp.title,
                        tmp.description,
                        tmp.answer,
                        tmp.createdDate,
                        tmp.updateDate,
                        tmp.status,
                        tmp.order
                    }
                ).OrderBy(x => x.createdDate)
                .ToList();

            return Json(new { success = true, questions = myQuestions });
        }
        
        [HttpGet("StudentQuestions/Details/{id}")]
        public async Task<JsonResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            
            var student = _context.Student.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (student == null)
            {
                return Json(new { success = false, message = "Student not found." });
            }
            
            var sessionIds = _context.StudentSignUps
                .Where(ss => ss.StudentId == student.Id)
                .Select(ss => ss.SessionId)
                .Distinct();
            
            var myQuestion = await _context.Questions
                .Join(
                    _context.StudentSignUps,
                    q => q.StudentSignUpId,
                    ssup => ssup.Id,
                    (q, ssup) => new { q, ssup }
                )
                .Where(x => x.q.id == id && sessionIds.Contains(x.ssup.SessionId))
                .Select(x => new
                {
                    x.q.id,
                    x.q.title,
                    x.q.description,
                    x.q.answer,
                    x.q.createdDate,
                    x.q.updateDate,
                    x.q.status,
                    x.q.order
                }).FirstOrDefaultAsync();
            
            if (myQuestion == null)
            {
                return Json(new { success = false, message = "Question not found or you don't have permission to view." });
            }

            return Json(new { success = true, question = myQuestion });
        }
        
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] StudentQuestionCreateViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            if (model == null)
            {
                return Json(new { success = false, message = "Invalid data received." });
            }

            if (String.IsNullOrEmpty(model.Date))
            {
                return Json(new { success = false, message = "Invalid data received." });
            }

            try
            {
                if(DateTime.TryParseExact(model.Date, "d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    var newQuestion = new Question
                    {
                        title = model.Title,
                        description = model.Description,
                        sessionDate = parsedDate,
                        createdDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        StudentSignUpId = model.StudentSignUpId
                    };
                    _context.Questions.Add(newQuestion);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Json(new { success = false, message = "Invalid date format." });
                }
                return Json(new { success = true, message = "Question created successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("StudentQuestions/{id}")]
        public async Task<JsonResult> UpdateQuestion(int id, [FromBody] StudentQuestionUpdateViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            
            var existingQuestion = await _context.Questions
                .FirstOrDefaultAsync(
                    q => q.id == id &&
                         q.StudentSignUp != null &&
                         q.StudentSignUp.Student.userId == currentUser.Id &&
                         (q.status == false || q.status == null)
                );

            if (existingQuestion == null)
            {
                return Json(new { success = false, message = "Question not found or you don't have permission to update." });
            }

            if (model == null || string.IsNullOrEmpty(model.Description))
            {
                return Json(new { success = false, message = "Invalid data received." });
            }

            try
            {
                existingQuestion.description = model.Description;
                existingQuestion.updateDate = DateTime.Now;

                _context.Questions.Update(existingQuestion);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Question updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        
        [HttpDelete("StudentQuestions/{id}")]
        public async Task<JsonResult> DeleteQuestion(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var existingQuestion = await _context.Questions
                .FirstOrDefaultAsync(
                    q => q.id == id &&
                         q.StudentSignUp != null &&
                         q.StudentSignUp.Student.userId == currentUser.Id &&
                         (q.status == false || q.status == null)
                         );

            if (existingQuestion == null)
            {
                return Json(new { success = false, message = "Question not found or you don't have permission to delete." });
            }

            try
            {
                _context.Questions.Remove(existingQuestion);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Question deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}

