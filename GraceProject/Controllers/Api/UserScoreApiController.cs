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
    public class UserScoreApiController : ControllerBase
    {
        private readonly GraceDbContext _context;

        public UserScoreApiController(GraceDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserScoresByUserId/{userId}")]
        public async Task<IActionResult> GetUserScoresByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "UserId is required." });
            }
            var scores = await _context.AppUserScores
                .Where(s => s.UserId == userId)
                .Select(s => new
                {
                    s.Id,
                    s.UserId,
                    s.AppGameLevelTaskId,
                    s.EarnedScore,
                    s.Description,
                    s.SavedDate
                })
                .ToListAsync();

            //if (scores == null || scores.Count == 0)
            //{
            //    return NotFound(new { message = "No scores found for the specified user." });
            //}

            return Ok(scores);
        }



        public async Task<bool> StoreUserScore(string userId, int? gameLevelTaskId, int? quizId, int earnedScore, string description)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(description) || earnedScore == 0)
            {
                return (false);
            }

            var newScore = new AppUserScore
            {
                UserId = userId,
                AppGameLevelTaskId = gameLevelTaskId,
                QuizId = quizId,
                EarnedScore = earnedScore,
                Description = description,
                SavedDate = DateTime.UtcNow
            };

            _context.AppUserScores.Add(newScore);
            await _context.SaveChangesAsync();

            return (true);
        }


        [HttpGet("SaveUserScore")]
        public async Task<IActionResult> SaveUserScore(
            [FromQuery] string UserId,
            [FromQuery] int? AppGameLevelTaskId,
            [FromQuery] int? QuizId,
            [FromQuery] int EarnedScore,
            [FromQuery] string Description
            )
        {

            var result = await StoreUserScore(UserId,
            AppGameLevelTaskId,
            QuizId,
            EarnedScore,
            Description);

            return Ok(result);
        }
    }
}

