using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BrightBoostApplication.Data;
using BrightBoostApplication.Models;
using AutoMapper;
using BrightBoostApplication.Models.ViewModel;


namespace BrightBoostApplication.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper? _mapper;

        public AttendanceController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetStudentAttendance(int sessionId, DateTime? AttendanceDateTime)
        {
            var studentSignups = _context.StudentSignups
                .Where(ss => ss.SessionId == sessionId).Include(s => s.Student).Select(tc=>new StudentSignUpViewModel {
                    StudentId = tc.Student.Id,
                    StudentName = _context.Users.Where(u => u.Id == tc.Student.userId).FirstOrDefault().firstName, 
                    StudentSignUpId == tc.Id,
                    StudentAttendanceStatus = _context.Attendance.Where(a => a.fkId == sessionId && a.AttendanceDateTime == AttendanceDateTime).FirstOrDefault()?.status ?? false
                })
                .ToList();
                ViewBag.AttendanceDateTime = AttendanceDateTime; 
                ViewBag.SessionId = sessionId;
                return View(studentSignups);

        }

        [HttpPost]
        public async Task<JsonResult> Create(List<AttendanceDto> Attendance)
        {
            if (attendanceList == null || attendanceList.Count == 0)
            {
                return BadRequest("No valid attendance data received.");
            }

            foreach (var attendanceDto in attendanceList)
                {
                    if (attendanceDto.AttendanceDateTime != default(DateTime))
                    {
                        var attendance = new Attendance
                        {
                            AttendanceDateTime = attendanceDto.AttendanceDateTime,
                            Status = attendanceDto.Status,
                            StudentSignUp = _context.StudentSignups.Find(attendanceDto.StudentSignUpId)
                        };
                        _context.Attendances.Add(attendance);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
                await _context.SaveChangesAsync();
                return Ok("Attendance records created successfully.");
        }
    }

    public class AttendanceDto
        {
        public DateTime AttendanceDateTime { get; set; }
        public bool Status { get; set; }
        public int StudentSignUpId { get; set; }
        }
}