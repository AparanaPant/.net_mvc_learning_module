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
    public class EquipmentApiController : ControllerBase
    {
        private readonly GraceDbContext _context;

        public EquipmentApiController(GraceDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetEquipmentsByGameLevel/{GameLevelId}")]
        public async Task<IActionResult> GetEquipmentsByGameLevel(int GameLevelId)
        {
            var equipments = await _context.AppEquipments
                .Where(e => e.AppGameLevelId == GameLevelId)
                .Select(e => new AppEquipment
                {
                    Id = e.Id,
                    Name = e.Name,
                    Resource3DModel = e.Resource3DModel,
                    Description = e.Description,
                    AppGameLevelId = e.AppGameLevelId,
                    App3DModelId = e.App3DModelId,
                    SavedDate = e.SavedDate,
                    RequiredScore = e.RequiredScore
                })
                .ToListAsync();

            //if (equipments == null || !equipments.Any())
            //{
            //    return NotFound(new { message = "No equipment found for this game level" });
            //}

            return Ok(equipments);
        }

        [HttpGet("GetEquipmentsBy3DModelId/{ModelId}")]
        public async Task<IActionResult> GetEquipmentsBy3DModelId(int ModelId)
        {
            var equipments = await _context.AppEquipments
                .Where(e => e.App3DModelId == ModelId)
                .Select(e => new AppEquipment
                {
                    Id = e.Id,
                    Name = e.Name,
                    Resource3DModel = e.Resource3DModel,
                    Description = e.Description,
                    AppGameLevelId = e.AppGameLevelId,
                    App3DModelId = e.App3DModelId,
                    SavedDate = e.SavedDate,
                    RequiredScore = e.RequiredScore
                })
                .ToListAsync();

            if (equipments == null || !equipments.Any())
            {
                return NotFound(new { message = "No equipment found for this game level" });
            }

            return Ok(equipments);
        }

        [HttpGet("GetEquipmentInfo/{EquipmentId}")]
        public async Task<IActionResult> GetEquipmentInfo(int EquipmentId)
        {
            var equipment = await _context.AppEquipments
                .Where(e => e.Id == EquipmentId)
                 .Select(e => new AppEquipment
                 {
                     Id = e.Id,
                     Name = e.Name,
                     Resource3DModel = e.Resource3DModel,
                     Description = e.Description,
                     AppGameLevelId = e.AppGameLevelId,
                     App3DModelId = e.App3DModelId,
                     SavedDate = e.SavedDate,
                     RequiredScore = e.RequiredScore
                 })
                .FirstOrDefaultAsync();

            if (equipment == null)
            {
                return NotFound(new { message = "Equipment or its 3D model not found" });
            }

            return Ok(equipment);
        }



    }
}


