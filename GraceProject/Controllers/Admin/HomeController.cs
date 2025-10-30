using GraceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GraceProject.Controllers.Student
{
    [Route("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this._userManager = userManager;
        }
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            if (!string.IsNullOrEmpty(_userManager.GetUserId(this.User)))
                return View("~/Views/Admin/Home/Dashboard.cshtml");
            else
                return NotFound();
        }
    }
}
