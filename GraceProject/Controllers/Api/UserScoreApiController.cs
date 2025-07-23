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
                    AppGameLevelTaskId = s.AppGameLevelTaskId ?? 0,
                    QuizId = s.QuizId ?? 0,
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

        [HttpGet("GetUserScoreByUserIdAndDescription")]
        public async Task<IActionResult> GetUserScoreByUserIdAndDescription(
            [FromQuery] string UserId,
            [FromQuery] string Description)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return BadRequest(new { message = "UserId is required." });
            }

            var scores = await _context.AppUserScores
                .Where(s => 
                s.Description.Trim().ToLower()== Description.Trim().ToLower() &&
                s.UserId==UserId
                )
                .Select(s => new
                {
                    s.Id,
                    s.UserId,
                    AppGameLevelTaskId = s.AppGameLevelTaskId ?? 0,
                    QuizId = s.QuizId ?? 0,
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



        public async Task<int> _GetTotalUserScore(string userId)
        {
            var totalScore = await _context.AppUserScores
                .Where(s => s.UserId == userId && s.QuizId != null)
                .GroupBy(s => new { s.UserId, s.QuizId })
                .Select(g => g.Max(s => s.EarnedScore))
                .SumAsync();

            return totalScore;
        }

        [HttpGet("GetTotalUserScore/{userId}")]
        public async Task<IActionResult> GetTotalUserScore(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "UserId is required." });
            }
            var result = await _GetTotalUserScore(userId);
            
            return Ok(result);
        }
    }
}

