using Microsoft.AspNetCore.Mvc;

namespace GraceProject.Controllers.Admin
{
    [Route("Admin/Teacher")]

    public class TeacherController : Controller
    {
        [Route("TeacherManagement")]

        public IActionResult TeacherManagement()
        {
            return View();
        }
    }
}
