using Microsoft.AspNetCore.Mvc;

[Route("AppSettings")]
public class AppSettingsController : Controller
{
    [Route("")]
    public IActionResult Index()
    {
        return View("~/Views/Application/index.cshtml");
    }
}
