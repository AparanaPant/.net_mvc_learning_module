using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using GraceProject.Models;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        if (!_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Login", "Account"); // Redirect to Login if not logged in
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin");
        }
        else if (roles.Contains("Educator"))
        {
            return RedirectToAction("Dashboard", "Educator");
        }
        else if (roles.Contains("Student"))
        {
            return RedirectToAction("Dashboard", "Student");
        }

        return RedirectToAction("Login", "Account"); // Fallback to login if no valid role
    }
}
