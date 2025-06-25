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
    public class ScoreApiController : ControllerBase
    {
        private readonly GraceDbContext _context;

        public ScoreApiController(GraceDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetScoreById/{ScoreId}")]
        public async Task<IActionResult> GetScoreById(int ScoreId)
        {
            var score = await _context.AppScores
                .Where(s => s.Id == ScoreId)
                .Select(s => new AppScore
                {
                    Id = s.Id,
                    Name = s.Name,
                    InitialScore = s.InitialScore
                })
                .FirstOrDefaultAsync();

            if (score == null)
            {
                return NotFound(new { message = "AppScore not found." });
            }

            return Ok(score);
        }
    }
}

