using System;
using Microsoft.AspNetCore.Mvc;
using BrightBoostApplication.Data;
using BrightBoostApplication.Models.ViewModel;
using System.Globalization;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace BrightBoostApplication.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminDashboard
        public async Task<IActionResult> Index()
        {
            var model = new StatisticsViewModel()
            {
                TotalUsers = _context.Users.Count(),
                TotalStudents = _context.Student.Count(),
                TotalCourseRegistrations = _context.StudentCourseSignUps.Count(),
                TotalSessionRegistrations = _context.StudentSignUps.Count(),
                registrationData = GetCourseRegistrationsByMonth(),
                QuestionStat = GetQuestionTrendByMonth()
            };

            return View(model);
        }

        public List<CourseRegistrationViewModel> GetCourseRegistrationsByMonth()
        {
            List<CourseRegistrationViewModel> registrationData = new List<CourseRegistrationViewModel>();
            var currentYear = DateTime.Now.Year;

            var courseSignUps = _context.StudentCourseSignUps
                .Where(signUp => signUp.createDate != null && signUp.createDate.Value.Year == currentYear)
                .ToList();

            var monthlyRegistrations = courseSignUps
                .GroupBy(signUp => signUp.createDate?.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    Count = group.Count()
                });

            for (int month = 1; month <= DateTime.Now.Month; month++)
            {
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                var registrationCount = monthlyRegistrations.FirstOrDefault(m => m.Month == month)?.Count ?? 0;

                registrationData.Add(new CourseRegistrationViewModel
                {
                    Month = monthName,
                    Registrations = registrationCount
                });
            }

            return registrationData;
        }

        public List<QuestionStatViewModel> GetQuestionTrendByMonth()
        {
            List<QuestionStatViewModel> registrationData = new List<QuestionStatViewModel>();
            var currentYear = DateTime.Now.Year;

            var questions = _context.Questions
                .Where(question => question.createdDate.Year == currentYear)
                .ToList();

            var monthlyQuuestions = questions
                .GroupBy(question => question.createdDate.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    Count = group.Count()
                });

            for (int month = 1; month <= DateTime.Now.Month; month++)
            {
                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                var count = monthlyQuuestions.FirstOrDefault(m => m.Month == month)?.Count ?? 0;

                registrationData.Add(new QuestionStatViewModel
                {
                    Month = monthName,
                    total = count
                });
            }

            return registrationData;
        }
    }
}
