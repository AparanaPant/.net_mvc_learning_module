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
    [Route("Student/Slides")]
    public class SlidesController : Controller
    {
        private readonly GraceDbContext _context;

        public SlidesController(GraceDbContext context)
        {
            _context = context;
        }

        [Route("Info")]
        // Get slides info by moduleid
        public async Task<IActionResult> Info(int? moduleid)
        {
            try
            {
                if (moduleid == null || _context.Slide == null)
                {
                    return NotFound();
                }

                var graceDbContext = _context.Slide.Include(s => s.ApplicationUser).Include(s => s.SlideSections).Where(s => s.ModuleId == moduleid).OrderBy(s => s.SlideOrder);
                return View("~/views/Student/Slides/Info.cshtml", await graceDbContext.ToListAsync());
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
