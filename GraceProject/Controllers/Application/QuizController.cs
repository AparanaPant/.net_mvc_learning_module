using GraceProject.Controllers.Api;
using GraceProject.Data;
using GraceProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


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
        var gameLevel = await _context.AppGameLevels.FindAsync(gameLevelId);
        if (gameLevel == null) return NotFound();

        // ✅ Always get the latest created quiz (regardless of due date or access)
        var latestQuiz = await _context.Quizzes
            .Where(q => q.GameLevelID == gameLevelId)
            .OrderByDescending(q => q.CreatedAt)
            .FirstOrDefaultAsync();
        Console.WriteLine($"[Debug] latestQuiz is {(latestQuiz != null ? "found" : "null")}");

        var latestQuizList = latestQuiz != null ? new List<Quiz> { latestQuiz } : new List<Quiz>();

        if (User.IsInRole("Student"))
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null) return Unauthorized();

           // bool canTakeQuiz = await CheckIfUserCanTakeQuiz(user.Id, gameLevel.Name);
            bool canTakeQuiz = true;

            ViewBag.GameLevelName = gameLevel.Name;
            ViewBag.CanTakeQuiz = canTakeQuiz;

            return View("~/Views/Application/StudentQuizList.cshtml", latestQuizList);
        }

        // Admin/Educator logic unchanged
        var allQuizzes = await _context.Quizzes
            .Where(q => q.GameLevelID == gameLevelId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        ViewBag.GameLevelID = gameLevelId;
        ViewBag.ArchivedQuizzes = allQuizzes.Where(q => q.IsArchived).ToList();

        return View("~/Views/Application/ViewGameLevelQuizzes.cshtml", allQuizzes);
    }



    private async Task<bool> CheckIfUserCanTakeQuiz(string userId, string gameLevelName)
    {
        var learningSlides = await _context.AppLearningSlides
            .Where(s => s.AppGameLevel.Name.ToLower().Trim() == gameLevelName.ToLower().Trim())
            .ToListAsync();

        var userTasks = await _context.AppUserTasksStatus
            .Where(t => t.UserId == userId)
            .ToListAsync();

        if (userTasks.Count == 0 || string.IsNullOrWhiteSpace(userTasks[0].TaskStatus))
            return false;

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(userTasks[0].TaskStatus);
        }
        catch
        {
            return false;
        }

        int numberOfDoneTasks = 0;
        JsonElement slides = doc.RootElement;

        foreach (var slide in slides.EnumerateArray())
        {
            foreach (var learnSlide in learningSlides)
            {
                if (learnSlide.Id == slide.GetProperty("LearningModuleSlideId").GetInt32())
                {
                    numberOfDoneTasks++;
                }
            }
        }

        return numberOfDoneTasks == learningSlides.Count;
    }


}
