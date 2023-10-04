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

namespace BrightBoostApplication.Controllers
{
    // [Authorize(Roles ="Tutor")]
    public class TutorQuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutorQuestionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            return _context.Questions != null ? 
                View() :
                Problem("Entity set 'ApplicationDbContext.Questions' is null.");
        }
        
        [HttpGet]
        public async Task<JsonResult> GetAllMySessionQuestionsAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            try
            {
                var allQuestions = await _context.Questions
                    .Join(
                        _context.StudentSignUps,
                        q => q.StudentSignUpId,
                        ss => ss.Id,
                        (q, ss) => new { q, ss }
                    )
                    .Join(
                        _context.Sessions,
                        tmp => tmp.ss.SessionId,
                        s => s.Id,
                        (tmp, s) => new { tmp.q, tmp.ss, s }
                    )
                    .Join(
                        _context.TutorAllocations,
                        tmp => tmp.s.Id,
                        ta => ta.SessionId,
                        (tmp, ta) => new { tmp.q, tmp.ss, tmp.s, ta }
                    )
                    .Join(
                        _context.Tutors,
                        tmp => tmp.ta.TutorId,
                        t => t.Id,
                        (tmp, t) => new { tmp.q, tmp.ss, tmp.s, tmp.ta, t }
                    )
                    .Where(tmp => tmp.t.userId == currentUser.Id)
                    .Select(
                        tmp => new
                        {
                            tmp.q.id,
                            tmp.q.title,
                            tmp.q.description,
                            tmp.q.answer,
                            tmp.q.createdDate,
                            tmp.q.updateDate,
                            tmp.q.status,
                            tmp.q.order,
                            tmp.s.SessionName
                        }
                    )
                    .ToListAsync();

                return Json(new { success = true, questions = allQuestions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        
        [HttpGet("TutorQuestions/{id}")]
        public async Task<JsonResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var studentQuestion = await _context.Questions
                .Join(
                    _context.StudentSignUps,
                    q => q.StudentSignUpId,
                    ss => ss.Id,
                    (q, ss) => new { q, ss }
                )
                .Join(
                    _context.Sessions,
                    tmp => tmp.ss.SessionId,
                    s => s.Id,
                    (tmp, s) => new { tmp.q, tmp.ss, s }
                )
                .Join(
                    _context.TutorAllocations,
                    tmp => tmp.s.Id,
                    ta => ta.SessionId,
                    (tmp, ta) => new { tmp.q, tmp.ss, tmp.s, ta }
                )
                .Join(
                    _context.Tutors,
                    tmp => tmp.ta.TutorId,
                    t => t.Id,
                    (tmp, t) => new { tmp.q, tmp.ss, tmp.s, tmp.ta, t }
                )
                .Where(
                    tmp => tmp.q.id == id &&
                           tmp.t.userId == currentUser.Id
                           )
                .Select(x => x.q)
                .FirstOrDefaultAsync();
            if (studentQuestion == null)
            {
                return Json(new { success = false, message = "Question not found or you don't have permission to view." });
            }

            return Json(new { success = true, question = studentQuestion });
        }
        
        [HttpPut("TutorQuestions/{id}")]
        public async Task<JsonResult> UpdateQuestionAsync(int id, [FromBody] TutorQuestionUpdateViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            try
            {
                var questionToUpdate = await _context.Questions
                    .Join(
                        _context.StudentSignUps,
                        q => q.StudentSignUpId,
                        ss => ss.Id,
                        (q, ss) => new { q, ss }
                    )
                    .Join(
                        _context.Sessions,
                        tmp => tmp.ss.SessionId,
                        s => s.Id,
                        (tmp, s) => new { tmp.q, tmp.ss, s }
                    )
                    .Join(
                        _context.TutorAllocations,
                        tmp => tmp.s.Id,
                        ta => ta.SessionId,
                        (tmp, ta) => new { tmp.q, tmp.ss, tmp.s, ta }
                    )
                    .Join(
                        _context.Tutors,
                        tmp => tmp.ta.TutorId,
                        t => t.Id,
                        (tmp, t) => new { tmp.q, tmp.ss, tmp.s, tmp.ta, t }
                    )
                    .Where(
                        tmp => tmp.q.id == id &&
                               tmp.t.userId == currentUser.Id &&
                               (tmp.q.status == false || tmp.q.status == null)
                    )
                    .Select(x => x.q)
                    .FirstOrDefaultAsync();

                if (questionToUpdate == null)
                {
                    return Json(new { success = false, message = "Question not found or cannot be updated." });
                }

                // Update only the answer
                questionToUpdate.answer = model.Answer;
                questionToUpdate.updateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Question answered successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        
        [HttpDelete("TutorQuestions/{id}")]
        public async Task<JsonResult> DeleteQuestionAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var existingQuestion = await _context.Questions
                .Join(
                    _context.StudentSignUps,
                    q => q.StudentSignUpId,
                    ss => ss.Id,
                    (q, ss) => new { q, ss }
                )
                .Join(
                    _context.Sessions,
                    tmp => tmp.ss.SessionId,
                    s => s.Id,
                    (tmp, s) => new { tmp.q, tmp.ss, s }
                )
                .Join(
                    _context.TutorAllocations,
                    tmp => tmp.s.Id,
                    ta => ta.SessionId,
                    (tmp, ta) => new { tmp.q, tmp.ss, tmp.s, ta }
                )
                .Join(
                    _context.Tutors,
                    tmp => tmp.ta.TutorId,
                    t => t.Id,
                    (tmp, t) => new { tmp.q, tmp.ss, tmp.s, tmp.ta, t }
                )
                .Where(
                    tmp => tmp.q.id == id &&
                           tmp.t.userId == currentUser.Id &&
                           (tmp.q.status == false || tmp.q.status == null)
                )
                .Select(x => x.q)
                .FirstOrDefaultAsync();

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
        
        [HttpPost]
        public async Task<JsonResult> BatchUpdateStatusAsync([FromBody] List<int> questionIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            try
            {
                var questionsToUpdate = await _context.Questions
                    .Join(
                        _context.StudentSignUps,
                        q => q.StudentSignUpId,
                        ss => ss.Id,
                        (q, ss) => new { Question = q, StudentSignUp = ss }
                    )
                    .Join(
                        _context.Sessions,
                        x => x.StudentSignUp.SessionId,
                        s => s.Id,
                        (x, s) => new { x.Question, x.StudentSignUp, Session = s }
                    )
                    .Join(
                        _context.TutorAllocations,
                        x => x.Session.Id,
                        ta => ta.SessionId,
                        (x, ta) => new { x.Question, x.StudentSignUp, x.Session, TutorAllocation = ta }
                    )
                    .Join(
                        _context.Tutors,
                        x => x.TutorAllocation.TutorId,
                        t => t.Id,
                        (x, t) => new { x.Question, x.StudentSignUp, x.Session, x.TutorAllocation, Tutor = t }
                    )
                    .Where(x => questionIds.Contains(x.Question.id)
                                && x.Tutor.userId == currentUser.Id
                                && (x.Question.status == false || x.Question.status == null)
                    )
                    .Select(x => x.Question)
                    .ToListAsync();

                if (questionsToUpdate.Count == 0)
                {
                    return Json(new { success = false, message = "No eligible questions to update." });
                }

                foreach (var question in questionsToUpdate)
                {
                    question.status = true;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Questions updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}