using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraceProject.Data;
using GraceProject.Models;
using System.Security.Claims;

namespace GraceProject.Controllers.Student
{
    [Route("Student/Modules")]
    public class ModulesController : Controller
    {
        private readonly GraceDbContext _context;

        public ModulesController(GraceDbContext context)
        {
            _context = context;
        }

        // GET: Modules
        [Route("Index/{courseId}")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string courseId)
        {
            var UserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

            ViewData["Course"] = _context.Course.FirstOrDefault(c => c.CourseID == courseId);

            var g1raceDbContext = _context.Enrollment
                .Where(e => e.StudentUserID == UserId && e.CourseID == courseId);
            IEnumerable<GraceProject.Models.Module> Modules = _context.Enrollment
                .Where(e => e.StudentUserID == UserId && e.CourseID== courseId)
                .SelectMany(e => e.Course.Modules) // Flatten Modules collection
                .ToList();
            return View("~/views/Student/Modules/Index.cshtml", Modules);
        }


    }
}
