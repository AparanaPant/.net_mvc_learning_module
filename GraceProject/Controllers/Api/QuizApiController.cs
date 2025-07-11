using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GraceProject.Models;
using GraceProject.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json;

namespace GraceProject.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizApiController : ControllerBase
    {
        private readonly GraceDbContext _context;

        public QuizApiController(GraceDbContext context)
        {
            _context = context;
        }

        // GET: api/QuizApi/app/{appQuizId}
        [HttpGet("GetAppQuiz/{GameLevelId}")]
        public async Task<IActionResult> GetAppQuiz(int GameLevelId)
        {
            var quiz = await _context.AppQuizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .Where(q => q.GameLevelId == GameLevelId)
                .Select(q => new QuizDto
                {
                    QuizId = q.AppQuizId,
                    Title = q.Title,
                    Questions = q.Questions.Select(qu => new QuestionDto
                    {
                        QuestionId = qu.AppQuestionId,
                        Text = qu.Text,
                        Points = qu.Points,
                        Options = qu.Options.Select(opt => new OptionDto
                        {
                            OptionId = opt.AppOptionId,
                            Text = opt.Text,
                            IsCorrect = opt.IsCorrect
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            //if (quiz == null)
            //{
            //    return NotFound(new { message = "AppQuiz not found" });
            //}

            return Ok(quiz);
        }

        public async Task<bool> CheckIfUserCanTakeQuiz(string userId, string gameLevelName)
        {
            var LearningSlides = await _context.AppLearningSlides
                .Where(s => s.AppGameLevel.Name.ToLower().Trim() == gameLevelName.ToLower().Trim())
                .Select(s => new AppLearningSlide
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    ImagePath = s.ImagePath,
                    App3DModelId = s.App3DModelId,
                    AppGameLevelId = s.AppGameLevelId
                })
                .ToListAsync();

            var UItasks = await _context.AppUserTasksStatus
               .Where(t => t.UserId == userId)
               .Select(t => new AppUserTasksStatus
               {
                   Id = t.Id,
                   AppGameLevelTaskId = t.AppGameLevelTaskId,
                   EarnedScore = t.EarnedScore,
                   TaskStatus = t.TaskStatus,
                   SavedDate = t.SavedDate,
                   UserId = t.UserId
               })
               .ToListAsync();

            if (UItasks.Count == 0 || string.IsNullOrWhiteSpace(UItasks[0].TaskStatus))
                return false;

            int NumberOfDoneTasks = 0;
            JsonDocument doc;

            try
            {
                doc = JsonDocument.Parse(UItasks[0].TaskStatus);
            }
            catch
            {
                return false;
            }

            JsonElement slides = doc.RootElement;

            for (int i = 0; i < slides.GetArrayLength(); i++)
            {
                var slide = slides[i];

                for (int j = 0; j < LearningSlides.Count; j++)
                {
                    if (LearningSlides[j].Id == slide.GetProperty("LearningModuleSlideId").GetInt32())
                    {
                        NumberOfDoneTasks++;
                    }
                }
            }

            return NumberOfDoneTasks == LearningSlides.Count;
        }




        [HttpGet("AllowUserTakePartInQuiz")]
        public async Task<IActionResult> AllowUserTakePartInQuiz(
            [FromQuery] string UserId,
            [FromQuery] string GameLevelName)
        {


            bool result = await CheckIfUserCanTakeQuiz(UserId, GameLevelName);
            return Ok(result);
            //var LearningSlides = await _context.AppLearningSlides
            //    .Where(s => s.AppGameLevel.Name.ToLower().Trim() == GameLevelName.ToLower().Trim())
            //    .Select(s => new AppLearningSlide
            //    {
            //        Id = s.Id,
            //        Title = s.Title,
            //        Description = s.Description,
            //        ImagePath = s.ImagePath,
            //        App3DModelId = s.App3DModelId,
            //        AppGameLevelId = s.AppGameLevelId
            //    })
            //    .ToListAsync();

            //var UItasks = await _context.AppUserTasksStatus
            //   .Where(t => t.UserId == UserId)
            //   .Select(t => new AppUserTasksStatus
            //   {
            //       Id = t.Id,
            //       AppGameLevelTaskId = t.AppGameLevelTaskId,
            //       EarnedScore = t.EarnedScore,
            //       TaskStatus = t.TaskStatus,
            //       SavedDate = t.SavedDate,
            //       UserId = t.UserId
            //   })
            //   .ToListAsync();

            //int NumberOfDoneTasks = 0;

            //string ReadSlides = UItasks[0].TaskStatus;
            //JsonDocument doc;
            //try
            //{
            //    doc = JsonDocument.Parse(ReadSlides);
            //}
            //catch (Exception ex)
            //{
            //    return Ok(false);
            //}

            //JsonElement slides = doc.RootElement;

            //for (int i = 0; i < slides.GetArrayLength(); i++)
            //{

            //    var slide = slides[i];

            //    for (int j = 0; j < LearningSlides.Count; j++)
            //    {
            //        if (LearningSlides[j].Id == slide.GetProperty("LearningModuleSlideId").GetInt32())
            //        {
            //            NumberOfDoneTasks++;
            //        }
            //    }
            //}

            //if (NumberOfDoneTasks == LearningSlides.Count)
            //    return Ok(true);
            //else
            //    return Ok(false);


        }
    }
}



public class QuizDto
{
    public int QuizId { get; set; }
    public string Title { get; set; }
    public List<QuestionDto> Questions { get; set; }
}

public class QuestionDto
{
    public int QuestionId { get; set; }
    public string Text { get; set; }
    public int Points { get; set; }
    public List<OptionDto> Options { get; set; }
}

public class OptionDto
{
    public int OptionId { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}
