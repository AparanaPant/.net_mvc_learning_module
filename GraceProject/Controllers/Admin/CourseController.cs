using GraceProject.Data;
using GraceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GraceProject.Controllers.Admin

{
    [Route("Admin/Courses")]
    public class CourseController : Controller
    {
        private readonly GraceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CourseController(GraceDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Course.ToListAsync();
            if (courses == null)
                return NotFound("No courses available.");

            // Pass the courses list to the view
            return View("~/Views/Admin/Courses/Courses.cshtml", courses);
        }

        [HttpGet]
        [Route("Manage/{courseId}")]
        public async Task<IActionResult> ManageCourse(string courseId)
        {
            var course = await _context.Course
                .Include(c => c.Modules)
                .Include(c => c.Quizzes)
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
                return NotFound();  // Return 404 if course not found

            // Fetch all educators based on role
            var educators = await _userManager.GetUsersInRoleAsync("Educator");
            ViewBag.Educators = educators;  // Pass educators to the view

            // Pass the course model to the view
            return View("~/Views/Admin/Courses/ManageCourse.cshtml", course);
        }


        [HttpPost]
        [Route("AddEducator")]
        [HttpPost]
        public async Task<IActionResult> AddEducatorToCourse(string educatorId, string courseId)
        {
            if (string.IsNullOrEmpty(educatorId) || string.IsNullOrEmpty(courseId))
                return BadRequest("Invalid input");

            var educatorService = new EducatorService(_context);
            var result = await educatorService.RegisterEducatorToCourse(educatorId, courseId);

            if (result)
                return RedirectToAction("ManageCourse", new { courseId });
            else
                return BadRequest("Failed to assign educator to the course");
        }
    }
}
