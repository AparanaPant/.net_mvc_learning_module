using Microsoft.AspNetCore.Mvc;

namespace GraceProject.Controllers.Report
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
