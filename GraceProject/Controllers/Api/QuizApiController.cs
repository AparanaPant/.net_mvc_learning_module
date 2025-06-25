using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GraceProject.Models;
using GraceProject.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
