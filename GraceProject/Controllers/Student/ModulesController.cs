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
            // Retrieve the course details
            ViewData["Course"] = _context.Course.FirstOrDefault(c => c.CourseID == courseId);

            // Fetch the modules for the course
            IEnumerable<GraceProject.Models.Module> Modules = _context.Module
                .Where(m => m.CourseId == courseId)
                .ToList();

            // Return the modules to the view
            return View("~/views/Student/Modules/Index.cshtml", Modules);
        }


    }
}
