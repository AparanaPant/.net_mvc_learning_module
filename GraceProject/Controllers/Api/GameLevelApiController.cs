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
    public class GameLevelApiController : ControllerBase
    {
        private readonly GraceDbContext _context;

        public GameLevelApiController(GraceDbContext context)
        {
            _context = context;
        }

        // GET: api/GameLevelApi/GetGameLevel/{GameLevelId}
        [HttpGet("GetGameLevelById/{GameLevelId}")]
        public async Task<IActionResult> GetGameLevelById(int GameLevelId)
        {
            var GameLevel = await _context.AppGameLevels
                .Where(GL => GL.Id == GameLevelId)
                .Select(GL => new AppGameLevel
                {
                    Id = GL.Id,
                    Name = GL.Name,
                    Description = GL.Description,
                    SavedDate = GL.SavedDate,
                    RequiredScore = GL.RequiredScore
                  
                })
                .FirstOrDefaultAsync();


            return Ok(GameLevel);
        }

        [HttpGet("GetGameLevelByName/{GameLevelName}")]
        public async Task<IActionResult> GetGameLevelByName(string GameLevelName)
        {
            var GameLevel = await _context.AppGameLevels
                .Where(GL => GL.Name.ToLower().Trim() == GameLevelName.ToLower().Trim())
                .Select(GL => new AppGameLevel
                {
                    Id = GL.Id,
                    Name = GL.Name,
                    Description = GL.Description,
                    SavedDate = GL.SavedDate,
                    RequiredScore = GL.RequiredScore

                })
                .FirstOrDefaultAsync();


            return Ok(GameLevel);
        }

    }
}



