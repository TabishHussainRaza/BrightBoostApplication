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
    [Authorize(Roles ="Tutor")]
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
        
        [HttpGet("TutorQuestions/GetAllMySessionQuestions")]
        public async Task<JsonResult> GetAllMySessionQuestionsAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            
            var tutor = _context.Tutors.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (tutor == null)
            {
                return Json(new { success = false, message = "Tutor not found." });
            }

            var sessionIds = _context.TutorAllocations
                .Where(ta => ta.TutorId == tutor.Id)
                .Select(ta => ta.SessionId).ToList();
            
            try
            {
                var allQuestions = await _context.Questions
                    .GroupJoin(
                        _context.StudentSignUps, 
                        q => q.StudentSignUpId,
                        ss => ss.Id,
                        (q, ss) => new { q, ss }
                    )
                    .SelectMany(
                        x => x.ss.DefaultIfEmpty(),
                        (x, ss) => new { x.q, ss }  
                    )
                    .GroupJoin(
                        _context.TutorAllocations,
                        x => x.q.TutorAllocationId, 
                        t => t.Id,
                        (x, t) => new { x.q, x.ss, t }
                    )
                    .SelectMany(
                        x => x.t.DefaultIfEmpty(),
                        (x, t) => new { x.q, x.ss, t }
                    )
                    .Where(
                        tmp => (tmp.ss != null && sessionIds.Contains(tmp.ss.SessionId)) ||
                               (tmp.t != null && sessionIds.Contains(tmp.t.SessionId))
                    )
                    .Select(
                        tmp => new {
                            tmp.q.id,
                            tmp.q.title,
                            tmp.q.description,
                            tmp.q.answer,
                            tmp.q.createdDate,
                            tmp.q.updateDate, 
                            tmp.q.status,
                            tmp.q.order
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
        
        [HttpGet("TutorQuestions/Details/{id}")]
        public async Task<JsonResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            
            var tutor = _context.Tutors.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (tutor == null)
            {
                return Json(new { success = false, message = "Tutor not found." });
            }
            
            var studentQuestion = await _context.Questions
                .Where(q => q.id == id).FirstOrDefaultAsync();
            if (studentQuestion == null)
            {
                return Json(new { success = false, message = "Question not found." });
            }

            if (studentQuestion.StudentSignUpId != null)
            {
                if (studentQuestion.StudentSignUp == null)
                {
                    return Json(new { success = false, message = "Question StudentSignUp doesn't exist." });
                }
                bool isMySessionQuestion = _context.TutorAllocations
                    .Any(
                        ta => ta.TutorId == tutor.Id &&
                              ta.SessionId == studentQuestion.StudentSignUp.SessionId
                    );
                if (!isMySessionQuestion)
                {
                    return Json(new { success = false,
                        message = "No permission to view the Question which not belongs to your session." }
                    );
                }
            }
            
            if (studentQuestion.TutorAllocationId != null)
            {
                if (studentQuestion.TutorAllocation == null)
                {
                    return Json(new { success = false, message = "Question TutorAllocation doesn't exist." });
                }
                bool isMySessionQuestion = _context.TutorAllocations
                    .Any(
                        ta => ta.TutorId == tutor.Id &&
                              ta.SessionId == studentQuestion.TutorAllocation.SessionId
                    );
                if (!isMySessionQuestion)
                {
                    return Json(new { success = false,
                        message = "No permission to view the Question which not belongs to your session." }
                    );
                }
            }
            return Json(new { success = true, question = studentQuestion });
        }
        
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] TutorQuestionCreateViewModel model)
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

            try
            {
                var newQuestion = new Question
                {
                    title = model.Title,
                    description = model.Description,
                    answer = model.Answer,
                    createdDate = DateTime.Now,
                    updateDate = DateTime.Now,
                    TutorAllocationId = model.TutorAllocationId
                };

                _context.Questions.Add(newQuestion);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Question created successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        
        [HttpPut("TutorQuestions/{id}")]
        public async Task<JsonResult> UpdateQuestionAsync(int id, [FromBody] TutorQuestionUpdateViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            
            var tutor = _context.Tutors.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (tutor == null)
            {
                return Json(new { success = false, message = "Tutor not found." });
            }
            
            var questionToUpdate = await _context.Questions.FirstOrDefaultAsync(q => q.id == id);
            if (questionToUpdate == null)
            {
                return Json(new { success = false, message = "Question not found." });
            }
            
            if (questionToUpdate.status == true)
            {
                return Json(new { success = false, message = "Question has been checked." });
            }

            if (questionToUpdate.StudentSignUpId != null)
            {
                if (questionToUpdate.StudentSignUp == null)
                {
                    return Json(new { success = false, message = "Question StudentSignUp doesn't exist." });
                }
                bool isMySessionQuestion = _context.TutorAllocations
                    .Any(
                        ta => ta.TutorId == tutor.Id &&
                              ta.SessionId == questionToUpdate.StudentSignUp.SessionId
                    );
                if (!isMySessionQuestion)
                {
                    return Json(new { success = false,
                        message = "No permission to answer the Question which not belongs to your session." }
                    );
                }
            }
            
            if (questionToUpdate.TutorAllocationId != null)
            {
                if (questionToUpdate.TutorAllocation == null)
                {
                    return Json(new { success = false, message = "Question TutorAllocation doesn't exist." });
                }
                bool isMyQuestion = questionToUpdate.TutorAllocation.TutorId == tutor.Id;
                if (!isMyQuestion)
                {
                    return Json(new { success = false,
                        message = "No permission to answer the Question created by another tutor." }
                    );
                }
            }

            try
            {
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
            
            var tutor = _context.Tutors.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (tutor == null)
            {
                return Json(new { success = false, message = "Tutor not found." });
            }
            
            var questionToDelete = await _context.Questions.FirstOrDefaultAsync(q => q.id == id);
            if (questionToDelete == null)
            {
                return Json(new { success = false, message = "Question not found." });
            }
            
            if (questionToDelete.status == true)
            {
                return Json(new { success = false, message = "Question has been checked." });
            }

            if (questionToDelete.StudentSignUpId != null)
            {
                return Json(new { success = false, message = "No permission to delete students' Question." });
            }
            
            if (questionToDelete.TutorAllocationId != null)
            {
                if (questionToDelete.TutorAllocation == null)
                {
                    return Json(new { success = false, message = "Question TutorAllocation doesn't exist." });
                }
                bool isMyQuestion = questionToDelete.TutorAllocation.TutorId == tutor.Id;
                if (!isMyQuestion)
                {
                    return Json(new { success = false,
                        message = "No permission to delete the Question created by another tutor." }
                    );
                }
            }

            try
            {
                _context.Questions.Remove(questionToDelete);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Question deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        
        [HttpPut("TutorQuestions/UpdateStatus/{id}")]
        public async Task<JsonResult> UpdateStatusAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            
            var tutor = _context.Tutors.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (tutor == null)
            {
                return Json(new { success = false, message = "Tutor not found." });
            }
            
            var questionToCheck = await _context.Questions.FirstOrDefaultAsync(q => q.id == id);
            if (questionToCheck == null)
            {
                return Json(new { success = false, message = "Question not found." });
            }
            
            if (questionToCheck.status == true)
            {
                return Json(new { success = false, message = "Question has been checked." });
            }

            if (questionToCheck.StudentSignUpId != null)
            {
                if (questionToCheck.StudentSignUp == null)
                {
                    return Json(new { success = false, message = "Question StudentSignUp doesn't exist." });
                }
                bool isMySessionQuestion = _context.TutorAllocations
                    .Any(
                        ta => ta.TutorId == tutor.Id &&
                              ta.SessionId == questionToCheck.StudentSignUp.SessionId
                    );
                if (!isMySessionQuestion)
                {
                    return Json(new { success = false,
                        message = "No permission to check the Question which not belongs to your session." }
                    );
                }
            }
            
            if (questionToCheck.TutorAllocationId != null)
            {
                if (questionToCheck.TutorAllocation == null)
                {
                    return Json(new { success = false, message = "Question TutorAllocation doesn't exist." });
                }
                bool isMyQuestion = questionToCheck.TutorAllocation.TutorId == tutor.Id;
                if (!isMyQuestion)
                {
                    return Json(new { success = false,
                        message = "No permission to check the Question created by another tutor." }
                    );
                }
            }

            try
            {
                questionToCheck.status = true;
                questionToCheck.updateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Questions checked successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}