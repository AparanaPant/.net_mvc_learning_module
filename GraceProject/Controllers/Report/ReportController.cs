using GraceProject.Models;
using GraceProject.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GraceProject.Controllers.Report
{
    [Route("Report/Report")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly GraceDbContext _context;

        public ReportController(GraceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View("~/Views/Report/AdminReport.cshtml");
        }

        // Get Student List
        [HttpPost("GetStudentList")]
        public async Task<IActionResult> GetStudentList([FromBody] SearchModel searchData)
        {
            // Get the Role ID for 'Student'
            var studentRoleId = await _context.Roles
                .Where(r => r.Name == "Student")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // If no student role is found, return empty list
            if (string.IsNullOrEmpty(studentRoleId))
            {
                return Ok(new List<object>());
            }

            // Fetch students by Role ID and match by name or email
            var students = await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == studentRoleId))
                .Where(u => u.FirstName.Contains(searchData.Keyword) ||
                            u.LastName.Contains(searchData.Keyword) ||
                            u.Email.Contains(searchData.Keyword))  // <-- Match by Email too
                .Select(u => new
                {
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,  // Include Email in the result
                    id = u.Id
                })
                .ToListAsync();

            // Log the result count for debugging
            Console.WriteLine("Students Found: " + students.Count);

            return Ok(students);
        }



        // Get Course List
        [HttpPost("GetCourseList")]
        public async Task<IActionResult> GetCourseList([FromBody] SearchModel searchData)
        {
            // Fetch all courses matching the keyword (if provided)
            var courses = await _context.Course
                .Where(c => string.IsNullOrEmpty(searchData.Keyword) || c.Title.Contains(searchData.Keyword))
                .Select(c => new
                {
                    name = c.Title + " (" + (c.Credits ?? 0) + " Credits)",  // Show title with credits in brackets
                    id = c.CourseID
                })
                .ToListAsync();

            // Log the result count for debugging
            Console.WriteLine("Courses Found: " + courses.Count);

            return Ok(courses);
        }


        // Get Sessions By Student
        // Get Sessions By Student
        [HttpPost("GetSessionsByStudent")]
        public IActionResult GetSessionsByStudent([FromBody] IdModel idModel)
        {
            var sessions = _context.StudentSessions
                .Where(ss => ss.StudentID == idModel.Id)
                .Select(ss => new
                {
                    name = ss.Session.Course.Title + " - " +
                           _context.Users
                               .Where(u => ss.Session.EducatorSessions.Any(es => es.EducatorID == u.Id))
                               .Select(u => u.FirstName + " " + u.LastName)
                               .FirstOrDefault(),
                    id = ss.SessionID
                })
                .ToList();

            return Ok(sessions);
        }


        // Get Sessions By Course
        // Get Sessions By Course
        [HttpPost("GetSessionsByCourse")]
        public IActionResult GetSessionsByCourse([FromBody] IdModel idModel)
        {
            var sessions = _context.Session
                .Where(s => s.CourseID == idModel.Id)
                .Select(s => new
                {
                    name = _context.Users
                               .Where(u => s.EducatorSessions.Any(es => es.EducatorID == u.Id))
                               .Select(u => u.FirstName + " " + u.LastName)
                               .FirstOrDefault() ?? "Instructor Not Assigned",
                    id = s.SessionID
                })
                .ToList();

            return Ok(sessions);
        }

    }

    // Models for AJAX
    public class SearchModel
    {
        public string Keyword { get; set; }
    }

    public class IdModel
    {
        public string Id { get; set; }
    }
}
