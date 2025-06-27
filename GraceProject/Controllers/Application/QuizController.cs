using GraceProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("Quiz")]
public class QuizController : Controller
{
    private readonly GraceDbContext _context;

    public QuizController(GraceDbContext context)
    {
        _context = context;
    }

    [HttpGet("SelectGameLevel")]
    public async Task<IActionResult> SelectGameLevel()
    {
        var levels = await _context.AppGameLevels
            .OrderBy(l => l.Name)
            .ToListAsync();

        return View("~/Views/Application/SelectGameLevel.cshtml", levels);
    }

    [HttpPost("SelectGameLevel")]
    public IActionResult SelectGameLevelPost(int gameLevelId)
    {
        return RedirectToAction("Create", "Quiz", new { gameLevelId });
    }

    [HttpPost("ViewGameLevelQuizzes")]
    public async Task<IActionResult> ViewGameLevelQuizzes(int gameLevelId)
    {
        var quizzes = await _context.Quizzes
            .Where(q => q.GameLevelID == gameLevelId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        ViewBag.GameLevelID = gameLevelId;
        ViewBag.ArchivedQuizzes = quizzes.Where(q => q.IsArchived).ToList();

        return View("~/Views/Application/ViewGameLevelQuizzes.cshtml", quizzes);

    }
}
