using GraceProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class QuizController : Controller
{
    private readonly GraceDbContext _context;

    public QuizController(GraceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("Quiz/SelectGameLevel")]
    public async Task<IActionResult> SelectGameLevel()
    {
        var levels = await _context.AppGameLevels
            .OrderBy(l => l.Name)
            .ToListAsync();

        return View("~/Views/Quiz/SelectGameLevel.cshtml", levels);
    }

    [HttpPost]
    [Route("Quiz/SelectGameLevel")]
    public IActionResult SelectGameLevelPost(int gameLevelId)
    {
        return RedirectToAction("Create", "Quiz", new { gameLevelId });
    }

}
