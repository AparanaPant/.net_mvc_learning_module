using GraceProject.Data;
using GraceProject.Models;
using GraceProject.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GraceProject.Controllers.Admin
{
    [Route("Admin/Courses")]
    public class CourseController : Controller
    {
        private readonly GraceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(GraceDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Course.ToListAsync();
            return View("~/Views/Admin/Courses/Courses.cshtml", courses);
        }

        [HttpGet]
        [Route("Manage/{courseId}")]
        public async Task<IActionResult> ManageCourse(string courseId)
        {
            var course = await _context.Course
                .Include(c => c.Modules)
                .Include(c => c.Quizzes)
                .Include(c => c.Sessions)
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
                return NotFound();

            var educators = await _userManager.GetUsersInRoleAsync("Educator");
            ViewBag.Educators = educators;

            return View("~/Views/Admin/Courses/ManageCourse.cshtml", course);
        }

        [HttpPost]
        [Route("CreateSessionForEducator")]
        public async Task<IActionResult> CreateSessionForEducator(string courseId, string educatorId)
        {
            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(educatorId))
                return BadRequest("Invalid input");

            var educatorService = new EducatorService(_context);
            var result = await educatorService.RegisterEducatorToCourse(educatorId, courseId);

            if (result)
            {
                // Fetch the latest session for the educator and course
                var session = await _context.Session
                    .Where(s => s.CourseID == courseId)
                    .OrderByDescending(s => s.SessionID) // Get the latest session created
                    .FirstOrDefaultAsync();

                if (session != null)
                {
                    // Redirect to the session details view
                    return Redirect($"/Admin/Courses/Sessions/List/{session.CourseID}");

                }
                else
                {
                    return BadRequest("Session created but could not retrieve session details.");
                }
            }
            else
            {
                return BadRequest("Failed to create session or educator is already assigned.");
            }
        }

        [HttpGet]
        [Route("Modules/Details/{moduleId}")]
        public async Task<IActionResult> ModuleDetails(int moduleId)
        {
            var module = await _context.Module.FindAsync(moduleId);
            if (module == null)
                return NotFound();

            return View("~/Views/Admin/Modules/Details.cshtml", module);
        }

        [HttpGet]
        [Route("Quizzes/Details/{quizId}")]
        public async Task<IActionResult> QuizDetails(int quizId)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
                return NotFound();

            return View("~/Views/Admin/Quizzes/Details.cshtml", quiz);
        }

        [HttpPost]
        [Route("AddCourse")]
        public async Task<IActionResult> AddCourse(string title, int credits)
        {
            if (string.IsNullOrEmpty(title) || credits <= 0)
            {
                return BadRequest("Invalid input");
            }

            // ✅ Auto-generate Course ID (e.g., C001, C002, ...)
            var lastCourse = await _context.Course.OrderByDescending(c => c.CourseID).FirstOrDefaultAsync();
            int nextId = lastCourse != null ? int.Parse(lastCourse.CourseID.Substring(1)) + 1 : 1;
            string newCourseId = "C" + nextId.ToString("D3");  // e.g., C001, C002, ...

            var newCourse = new GraceProject.Models.Course
            {
                CourseID = newCourseId,
                Title = title,
                Credits = credits
            };

            _context.Course.Add(newCourse);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet]
        [Route("Sessions/List/{courseId}")]
        public async Task<IActionResult> SessionsList(string courseId)
        {
            // ✅ Get all sessions for the course with Educator details using explicit joins
            var sessions = await _context.Session
                .Where(s => s.CourseID == courseId)  // Fetch all sessions without filtering by IsActive
                .Include(s => s.EducatorSessions)
                .ThenInclude(es => es.Educator)  // Ensure Educator is loaded
                .OrderByDescending(s => s.SessionID)  // ✅ Sort by latest created session
                .Select(s => new
                {
                    s.SessionID,
                    s.StartDate,
                    s.EndDate,
                    s.IsActive,
                    // Fetch Educator names using sub-query approach
                    EducatorName = _context.Users
                        .Where(u => s.EducatorSessions.Any(es => es.EducatorID == u.Id))
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // ✅ Map Session to SessionViewModel
            var sessionList = sessions.Select(s => new GraceProject.Models.ViewModels.SessionViewModel
            {
                SessionID = s.SessionID,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                IsActive = s.IsActive,
                EducatorName = !string.IsNullOrEmpty(s.EducatorName) ? s.EducatorName : "Instructor Not Assigned"
            }).ToList();

            ViewBag.CourseID = courseId;  // Pass courseId to view

            // ✅ Fetch all educators and pass to ViewBag
            var educatorRoleId = await _context.Roles
                .Where(r => r.Name == "Educator")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            ViewBag.Educators = await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == educatorRoleId))
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email
                })
                .ToListAsync();
            return View("~/Views/Admin/Courses/SessionList.cshtml", sessionList);  // Pass view model
        }


        [HttpPost]
        [Route("Sessions/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(int sessionId, bool isActive)
        {
            var session = await _context.Session.FirstOrDefaultAsync(s => s.SessionID == sessionId);
            if (session != null)
            {
                session.IsActive = isActive;  // Set status based on checkbox
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Session not found.");
        }


        [HttpGet]
        [Route("Sessions/Details/{sessionId}")]
        public async Task<IActionResult> SessionDetails(int sessionId)
        {
            var session = await _context.Session
                .Include(s => s.StudentSessions)
                .ThenInclude(ss => ss.Student)
                .Include(s => s.EducatorSessions)
                .ThenInclude(es => es.Educator)
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.SessionID == sessionId);

            if (session == null)
                return NotFound();

            return View("~/Views/Admin/Sessions/Details.cshtml", session);
        }

        [HttpGet]
        [Route("Quizzes/List/{courseId}")]
        public async Task<IActionResult> QuizzesList(string courseId)
        {
            var quizzes = await _context.Quizzes.Where(q => q.CourseID == courseId).ToListAsync();
            ViewBag.CourseID = courseId;
            return View("~/Views/Admin/Courses/QuizList.cshtml", quizzes);
        }

        [HttpGet]
        [Route("Modules/List/{courseId}")]
        public async Task<IActionResult> ModulesList(string courseId)
        {
            var modules = await _context.Module.Where(m => m.CourseId == courseId).ToListAsync();

            ViewBag.CourseId = courseId; // Pass courseId to ViewBag

            return View("~/Views/Admin/Courses/ModuleList.cshtml", modules);
        }

    }
}