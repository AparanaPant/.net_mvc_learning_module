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
    public class GameLevelTasksApiController : ControllerBase
    {
        private readonly GraceDbContext _context;

        public GameLevelTasksApiController(GraceDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetTasksByGameLevelId/{GameLevelId}")]
        public async Task<IActionResult> GetTasksByGameLevelId(int GameLevelId)
        {
            var tasks = await _context.AppGameLevelTasks
                .Where(task => task.AppGameLevelId == GameLevelId)
                .Select(task => new AppGameLevelTask
                {
                    Id = task.Id,
                    AppGameLevelId = task.AppGameLevelId,
                    Title = task.Title,
                    Description = task.Description,
                    SavedDate = task.SavedDate,
                    RequiredEarningScore = task.RequiredEarningScore,
                    Task = task.Task
                    // Do not include navigation properties like AppUserTasksStatus unless you specifically want them
                })
                .ToListAsync();

            //if (tasks == null || !tasks.Any())
            //{
            //    return NotFound(new { message = "No tasks found for this Game Level" });
            //}

            return Ok(tasks);
        }
        [HttpGet("GetTasksByGameLevelName/{GameLevelName}")]
        public async Task<IActionResult> GetTasksByGameLevelName(string GameLevelName)
        {
            var tasks = await _context.AppGameLevelTasks
                .Where(task => task.AppGameLevel.Name.ToLower().Trim() == GameLevelName.ToLower().Trim())
                .Select(task => new AppGameLevelTask
                {
                    Id = task.Id,
                    AppGameLevelId = task.AppGameLevelId,
                    Title = task.Title,
                    Description = task.Description,
                    SavedDate = task.SavedDate,
                    RequiredEarningScore = task.RequiredEarningScore,
                    Task = task.Task
                    // Do not include navigation properties like AppUserTasksStatus unless you specifically want them
                })
                .ToListAsync();

            //if (tasks == null || !tasks.Any())
            //{
            //    return NotFound(new { message = "No tasks found for this Game Level" });
            //}

            return Ok(tasks);
        }

    }
}

