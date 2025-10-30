using GraceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GraceProject.Controllers.Guest
{
    [Route("Guest")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            if (!string.IsNullOrEmpty(_userManager.GetUserId(this.User)))
                return View("~/Views/Guest/Home/Dashboard.cshtml");
            else
                return NotFound();
        }
    }
}
