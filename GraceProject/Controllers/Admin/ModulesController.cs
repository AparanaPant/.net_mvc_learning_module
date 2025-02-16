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

namespace GraceProject.Controllers.Admin
{
    [Route("Admin/Modules")]
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
            var Module = _context.Module.Include(m => m.ApplicationUser).ToList();
            return View("~/views/Admin/Modules/Index.cshtml", Module);
        }

        // GET: Modules/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var @module = await _context.Module
                .Include(m => m.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View("~/views/Admin/Modules/Details.cshtml", @module);
        }

        // GET: Modules/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["ModuleList"] = _context.Module.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.ModuleName
            }).ToList();

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View("~/views/Admin/Modules/Create.cshtml");
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("Id,UserId,ModuleName,ParentModuleId,SavedDateTime")] Module @module)
        {
            @module.SavedDateTime = System.DateTime.Now;
            @module.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (module.UserId == null)
            { return NotFound(); }

            _context.Add(@module);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", @module.UserId);
            return View("~/views/Admin/Modules/Create.cshtml", @module);
        }

        // GET: Modules/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var @module = await _context.Module.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }

            ViewData["ModuleList"] = _context.Module.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.ModuleName
            }).ToList();

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", @module.UserId);
            return View("~/views/Admin/Modules/Edit.cshtml", @module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ModuleName,ParentModuleId,SavedDateTime")] Module @module)
        {
            if (id != @module.Id)
            {
                return NotFound();
            }

            if (module.Id == module.ParentModuleId)
                return BadRequest();

            try
            {
                _context.Update(@module);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(@module.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", @module.UserId);
            return View("~/views/Admin/Modules/Edit.cshtml", @module);
        }

        // GET: Modules/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Module == null)
            {
                return NotFound();
            }

            var @module = await _context.Module
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View("~/views/Admin/Modules/Delete.cshtml", @module);
        }

        // POST: Modules/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Module == null)
            {
                return Problem("Entity set 'GraceDbContext.Module'  is null.");
            }
            var @module = await _context.Module.FindAsync(id);
            if (@module != null)
            {
                _context.Module.Remove(@module);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return (_context.Module?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
