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
                Problem("Entity set 'ApplicationDbContext.Questions'  is null.");
        }
        
        [HttpGet]
        public async Task<JsonResult> GetAllMyQuestionsAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var myQuestions = _context.Questions
                .Where(q => q.StudentSignUp != null && q.StudentSignUp.Student.userId == currentUser.Id)
                .Select(q => new
                {
                    q.id,
                    q.title,
                    q.description,
                    q.answer,
                    q.createdDate,
                    q.updateDate,
                    q.status,
                    q.order,
                    q.StudentSignUpId
                })
                .ToList();

            return Json(new { success = true, questions = myQuestions });
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

            try
            {
                var newQuestion = new Question
                {
                    title = model.Title,
                    description = model.Description,

                    // TODO 'answer' column has not confirmed
                    // TODO 'status' column has not confirmed
                    // TODO 'order' column has not confirmed

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

    }
}

