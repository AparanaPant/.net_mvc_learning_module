// Controllers/StudentController.cs
using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult StudentReport()
        {
            return View();
        }
    }
}
