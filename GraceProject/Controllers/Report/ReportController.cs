using GraceProject.Models;
using GraceProject.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GraceProject.Models;


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

        [HttpPost("GetEducatorList")]
        public async Task<IActionResult> GetEducatorList([FromBody] SearchModel searchData)
        {
            // Fetch educator role ID
            var educatorRoleId = await _context.Roles
                .Where(r => r.Name == "Educator")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // If educator role is not found, return empty list
            if (string.IsNullOrEmpty(educatorRoleId))
            {
                return Ok(new List<object>());
            }

            // Fetch educators based on role ID and search keyword
            var educators = await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == educatorRoleId))
                .Where(u => u.FirstName.Contains(searchData.Keyword) || u.LastName.Contains(searchData.Keyword) || u.Email.Contains(searchData.Keyword))
                .Select(u => new
                {
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    id = u.Id
                })
                .ToListAsync();

            // Log result for debugging
            Console.WriteLine("Educators Found: " + educators.Count);

            return Ok(educators);
        }


        [HttpPost("GetSessionsByEducator")]
        public IActionResult GetSessionsByEducator([FromBody] IdModel idModel)
        {
            var sessions = _context.EducatorSession
                .Where(es => es.EducatorID == idModel.Id)
                .Select(es => new { name = es.Session.Course.Title, id = es.SessionID })
                .ToList();

            return Ok(sessions);
        }

        [HttpPost("GetStudentGrades")]
        public async Task<IActionResult> GetStudentGrades([FromBody] StudentSessionModel model)
        {
            // model.Id -> Session ID
            // model.Keyword -> Student ID

            var quizResults = await _context.UserQuizzes
                .Where(uq => uq.UserId == model.Keyword && uq.Quiz.SessionID == model.Id)
                .Select(uq => new
                {
                    QuizTitle = uq.Quiz.Title,
                    Score = uq.Score ?? 0,
                    FullMarks = uq.Quiz.TotalScore ?? 0
                })
                .ToListAsync();

            int totalScore = quizResults.Sum(q => q.Score);
            int totalFullMarks = quizResults.Sum(q => q.FullMarks);

            var result = new
            {
                QuizResults = quizResults,
                TotalScore = totalScore,
                TotalFullMarks = totalFullMarks
            };

            return Ok(result);
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
