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

        [HttpGet("SaveUserScore")]
        public async Task<IActionResult> SaveUserScore(
            [FromQuery] string UserId,
            [FromQuery] int AppGameLevelTaskId,
            [FromQuery] int EarnedScore,
            [FromQuery] string Description
            )
        {

            AppUserScore newScore =new AppUserScore();

            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Description) || EarnedScore==0)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            return Ok(new { message = "User score inserted successfully" });

            newScore.SavedDate = DateTime.UtcNow; // You can also accept it from client if needed
            newScore.AppGameLevelTaskId = AppGameLevelTaskId>0? AppGameLevelTaskId:null;
            newScore.EarnedScore = EarnedScore;
            newScore.Description = Description;
            newScore.UserId = UserId;
            return Ok(new { message = "User score inserted successfully" });

            _context.AppUserScores.Add(newScore);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User score inserted successfully", newScore.Id });
        }
    }
}

