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
        public IActionResult Index(int sessionId, DateTime? AttendanceDateTime)
        {
            var studentSignups = _context.StudentSignUps
                .Where(ss => ss.SessionId == sessionId).Include(s => s.Student).Select(tc => new StudentSignUpViewModel
                {
                    StudentId = tc.Student.Id,
                    StudentName = _context.Users.Where(u => u.Id == tc.Student.userId).FirstOrDefault().firstName,
                    StudentSignUpId = tc.Id,
                    StudentAttendanceStatus = _context.Attendances.Where(a => a.fkId == tc.Student.Id && a.AttendanceDateTime == AttendanceDateTime).FirstOrDefault().status ?? false
                })
                .ToList();
            ViewBag.AttendanceDateTime = AttendanceDateTime;
            ViewBag.SessionId = sessionId;
            return View(studentSignups);

        }

        [HttpPost]
        public async Task<JsonResult> Create(AttendanceListVM Attendance)
        {
            if (Attendance.sessionId == null || Attendance.date == null)
            {
                return Json(new { status = false, message = $"No Date Provided" });
            }

            foreach (var student in Attendance.studentData)
            {
                var studentData = _context.Student.Where(s=>s.Id == student.studentId).FirstOrDefault();
                var studentSignUp = _context.StudentSignUps.Where(st=>st.StudentId == student.studentId && st.SessionId==Attendance.sessionId).FirstOrDefault();
                if (studentData != null && studentSignUp != null)
                {
                    var prevAtt = _context.Attendances.Where(a=>a.AttendanceDateTime == Attendance.date && a.fkId== studentData.Id).FirstOrDefault();
                    if(prevAtt == null)
                    {
                        var att = new Attendance()
                        {
                            AttendanceDateTime = Attendance.date,
                            createdDate = DateTime.Now,
                            updateDate = DateTime.Now,
                            status = student.attendanceStatus,
                            fkId = studentData.Id,
                            StudentSignUp = studentSignUp,
                        };
                        _context.Add(att);
                    }
                    else
                    {
                        prevAtt.status = student.attendanceStatus;
                    }
                }
                else
                {
                    return Json(new { status = false, message = $"Student Information is Missing" });
                }
                await _context.SaveChangesAsync();
            }
            return Json(new { status = true, message = $"Attendance Updated" });

        }
    }
}