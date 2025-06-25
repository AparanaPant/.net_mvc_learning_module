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
    public class UserTasksStatusApiController : ControllerBase
    {
        private readonly GraceDbContext _context;

        public UserTasksStatusApiController(GraceDbContext context)
        {
            _context = context;
        }

        [HttpGet("SaveUserTaskStatus")]
        public async Task<IActionResult> SaveUserTaskStatus(
            [FromQuery] string userId,
            [FromQuery] int appGameLevelTaskId,
            [FromQuery] int earnedScore,
            [FromQuery] string taskStatus)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskStatus))
            {
                return BadRequest(new { message = "Missing required parameters." });
            }

            // Optional: Validate foreign keys
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            var taskExists = await _context.AppGameLevelTasks.AnyAsync(t => t.Id == appGameLevelTaskId);

            if (!userExists || !taskExists)
            {
                return NotFound(new { message = "User or Task not found." });
            }

            var newStatus = new AppUserTasksStatus
            {
                UserId = userId,
                AppGameLevelTaskId = appGameLevelTaskId,
                EarnedScore = earnedScore,
                TaskStatus = taskStatus,
                SavedDate = DateTime.UtcNow
            };

            _context.AppUserTasksStatus.Add(newStatus);
            await _context.SaveChangesAsync();

            return Ok(new { isdone=true, message = "User task status saved successfully", newStatus.Id });
        }


        [HttpGet("UpdateUserTaskStatus")]
        public async Task<IActionResult> UpdateUserTaskStatus(
    [FromQuery] int UserTaskStatusId,
    [FromQuery] int earnedScore,
    [FromQuery] string taskStatus)
        {
            // Validate required fields
            if (!(UserTaskStatusId>0) || string.IsNullOrEmpty(taskStatus))
            {
                return BadRequest(new { message = "Missing required parameters." });
            }

            // Optional: Validate foreign keys
            var userTaskStatus = await _context.AppUserTasksStatus.FirstOrDefaultAsync(t => t.Id == UserTaskStatusId);

            if (userTaskStatus==null)
            {
                return NotFound(new { message = "Task not found." });
            }

            //update
            userTaskStatus.EarnedScore = earnedScore;
            userTaskStatus.TaskStatus = taskStatus;

            // Save changes
            await _context.SaveChangesAsync();


            return Ok(new { isdone=true, message = "User task status saved successfully" });
        }



        [HttpGet("GetUserTaskStatusByTaskId/{TaskId}")]
        public async Task<IActionResult> GetUserTaskStatusByTaskId(int TaskId)
        {
            var taskstatus = await _context.AppUserTasksStatus
                .Where(t => t.AppGameLevelTaskId == TaskId)
                .Select(t => new AppUserTasksStatus
                {
                    Id = t.Id,
                   AppGameLevelTaskId=t.AppGameLevelTaskId,
                   EarnedScore=t.EarnedScore,
                   TaskStatus = t.TaskStatus,
                   SavedDate = t.SavedDate,
                   ApplicationUser = t.ApplicationUser
                })
                .ToListAsync();

            //if (taskstatus == null || taskstatus.Count == 0)
            //{
            //    return NotFound(new { message = "No learning slides found for the specified 3D model." });
            //}

            return Ok(taskstatus.FirstOrDefault());
        }


        [HttpGet("GetUserTaskStatusByUserId/{UserId}")]
        public async Task<IActionResult> GetUserTaskStatusByUserId(string UserId)
        {
            var taskstatus = await _context.AppUserTasksStatus
                .Where(t => t.UserId == UserId)
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

            //if (taskstatus == null || taskstatus.Count == 0)
            //{
            //    return NotFound(new { message = "No learning slides found for the specified 3D model." });
            //}

            return Ok(taskstatus);
        }

    }
}

