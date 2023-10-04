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
    // [Authorize(Roles ="Student")]
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
        public async Task<IActionResult> Index()
        {
            return _context.Questions != null ? 
                View() :
                Problem("Entity set 'ApplicationDbContext.Questions' is null.");
        }
        
        [HttpGet("StudentQuestions/GetAllMyQuestions")]
        public async Task<JsonResult> GetAllMyQuestionsAsync()
        {
            // TODO just for debugging, remember to change back 
            // var currentUser = await _userManager.GetUserAsync(User);
            //
            // if (currentUser == null)
            // {
            //     return Json(new { success = false, message = "User not found." });
            // }

            // var myQuestions = await _context.Questions
            //     .Join(
            //         _context.StudentSignUps,
            //         q => q.StudentSignUpId,
            //         ss => ss.Id,
            //         (q, ss) => new { q, ss }
            //     )
            //     .Join(
            //         _context.Student,
            //         x => x.ss.StudentId,
            //         st => st.Id,
            //         (x, st) => new { x.q, x.ss, st }
            //     )
            //     .Join(
            //         _context.Sessions,
            //         x => x.ss.SessionId,
            //         se => se.Id,
            //         (x, se) => new { x.q, x.ss, x.st, se }
            //     )
            //     .Where(x => x.st.userId == currentUser.Id)
            //     .Select(
            //         tmp => new
            //         {
            //             tmp.q.id,
            //             tmp.q.title,
            //             tmp.q.description,
            //             tmp.q.answer,
            //             tmp.q.createdDate,
            //             tmp.q.updateDate,
            //             tmp.q.status,
            //             tmp.q.order,
            //             tmp.se.SessionName
            //         }
            //     )
            //     .ToListAsync();
            
            // TODO just for debugging, remember to delete these code
            var myQuestions = await _context.Questions
                .Join(
                    _context.StudentSignUps,
                    q => q.StudentSignUpId,
                    ss => ss.Id,
                    (q, ss) => new { q, ss }
                )
                .Join(
                    _context.Student,
                    x => x.ss.StudentId,
                    st => st.Id,
                    (x, st) => new { x.q, x.ss, st }
                )
                .Join(
                    _context.Sessions,
                    x => x.ss.SessionId,
                    se => se.Id,
                    (x, se) => new { x.q, x.ss, x.st, se }
                )
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
                        tmp.se.SessionName
                    }
                )
                .ToListAsync();

            return Json(new { success = true, questions = myQuestions });
        }
        
        [HttpGet("StudentQuestions/Details/{id}")]
        public async Task<JsonResult> Details(int id)
        {
            // TODO just for debugging, remember to change back 
            // var currentUser = await _userManager.GetUserAsync(User);
            //
            // if (currentUser == null)
            // {
            //     return Json(new { success = false, message = "User not found." });
            // }

            // var myQuestion = await _context.Questions
            //     .FirstOrDefaultAsync(
            //         q => q.id == id &&
            //              q.StudentSignUp != null &&
            //              q.StudentSignUp.Student.userId == currentUser.Id
            //     );
            
            // TODO just for debugging, remember to delete these code
            var myQuestion = await _context.Questions
                .FirstOrDefaultAsync(
                    q => q.id == id
                );
            
            if (myQuestion == null)
            {
                return Json(new { success = false, message = "Question not found or you don't have permission to view." });
            }

            return Json(new { success = true, question = myQuestion });
        }
        
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] StudentQuestionCreateViewModel model)
        {
            // TODO just for debugging, remember to change back
            // var currentUser = await _userManager.GetUserAsync(User);
            //
            // if (currentUser == null)
            // {
            //     return Json(new { success = false, message = "User not found." });
            // }

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
                    createdDate = DateTime.Now,
                    updateDate = DateTime.Now,
                    StudentSignUpId = model.StudentSignUpId
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

        [HttpPut("StudentQuestions/{id}")]
        public async Task<JsonResult> UpdateQuestion(int id, [FromBody] StudentQuestionUpdateViewModel model)
        {
            // TODO just for debugging, remember to change back 
            // var currentUser = await _userManager.GetUserAsync(User);
            //
            // if (currentUser == null)
            // {
            //     return Json(new { success = false, message = "User not found." });
            // }
            
            // var existingQuestion = await _context.Questions
            //     .FirstOrDefaultAsync(
            //         q => q.id == id &&
            //              q.StudentSignUp != null &&
            //              q.StudentSignUp.Student.userId == currentUser.Id &&
            //              (q.status == false || q.status == null)
            //     );


            // TODO just for debugging, remember to delete these code
            var existingQuestion = await _context.Questions
                .FirstOrDefaultAsync(
                    q => q.id == id &&
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
            // TODO just for debugging, remember to change back
            // var currentUser = await _userManager.GetUserAsync(User);
            //
            // if (currentUser == null)
            // {
            //     return Json(new { success = false, message = "User not found." });
            // }

            // var existingQuestion = await _context.Questions
            //     .FirstOrDefaultAsync(
            //         q => q.id == id &&
            //              q.StudentSignUp != null &&
            //              q.StudentSignUp.Student.userId == currentUser.Id &&
            //              (q.status == false || q.status == null)
            //              );
            
            // TODO just for debugging, remember to delete these code
            var existingQuestion = await _context.Questions
                .FirstOrDefaultAsync(
                    q => q.id == id &&
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

