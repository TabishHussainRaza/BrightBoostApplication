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
using BrightBoostApplication.Data.Migrations;

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
        public async Task<IActionResult> Index(int SessionId, DateTime? SessionDateTime)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Unauthorized();
            }
            var tutor = _context.Tutors.Where(s => s.userId == currentUser.Id).FirstOrDefault();
            if (tutor == null)
            {
                return Unauthorized();
            }
            var tutorSignUp = _context.TutorAllocations.Where(s => s.SessionId == SessionId && s.TutorId == tutor.Id).FirstOrDefault();
            if (tutorSignUp == null)
            {
                return Unauthorized();
            }
            ViewBag.SessionDateTime = SessionDateTime;
            ViewBag.SessionId = SessionId;
            ViewBag.TutorAllId = tutorSignUp.Id;
            return View();
        }

        [HttpGet("TutorQuestions/GetAllMySessionQuestions")]
        public async Task<JsonResult> GetAllMySessionQuestionsAsync(int SessionId, string SessionDateTime)
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

            DateTime.TryParseExact(SessionDateTime, "d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

            var session = _context.Sessions.Where(s => s.Id == SessionId).FirstOrDefault();

            try
            {
                var myStudentQuestions = _context.Questions
                .Where(x => (x.StudentSignUp.SessionId == session.Id) && x.sessionDate == parsedDate).Include(q => q.StudentSignUp)
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

                var myQuestions = _context.Questions
                .Where(x => (x.TutorAllocation.SessionId == session.Id) && x.sessionDate == parsedDate).Include(q => q.TutorAllocation)
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

                return Json(new { success = true, questions = myStudentQuestions.Concat(myQuestions) });
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
                if (DateTime.TryParseExact(model.Date, "d/M/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    var newQuestion = new Question
                    {
                        title = model.Title,
                        description = model.Description,
                        answer = model.Answer,
                        sessionDate = parsedDate,
                        createdDate = DateTime.Now,
                        updateDate = DateTime.Now,
                        TutorAllocationId = model.TutorAllocationId
                    };

                    if (!string.IsNullOrEmpty(model.Answer))
                    {
                        newQuestion.status = true;
                    }
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

            try
            {
                // Update only the answer
                questionToUpdate.answer = model.Answer;

                if(!string.IsNullOrEmpty(questionToUpdate.answer))
                {
                    questionToUpdate.status = true;
                }
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