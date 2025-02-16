using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraceProject.Data;
using GraceProject.Models;

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
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var graceDbContext = _context.Module.Include(m => m.ApplicationUser);
            return View("~/views/Student/Modules/Index.cshtml", await graceDbContext.ToListAsync());
        }

       
    }
}
