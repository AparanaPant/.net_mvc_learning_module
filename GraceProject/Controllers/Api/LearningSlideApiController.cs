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
    public class LearningSlideApiController : ControllerBase
    {
        private readonly GraceDbContext _context;

        public LearningSlideApiController(GraceDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetSlidesBy3DModel/{App3DModelId}")]
        public async Task<IActionResult> GetSlidesBy3DModel(int App3DModelId)
        {
            var slides = await _context.AppLearningSlides
                .Where(s => s.App3DModelId == App3DModelId)
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

            if (slides == null || slides.Count == 0)
            {
                return NotFound(new { message = "No learning slides found for the specified 3D model." });
            }

            return Ok(slides);
        }

        [HttpGet("GetSlideInfoBySlideId/{SlideId}")]
        public async Task<IActionResult> GetSlideInfoBySlideId(int SlideId)
        {
            var slide = await _context.AppLearningSlides
                .Where(s => s.Id == SlideId)
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

            //if (slides == null || slides.Count == 0)
            //{
            //    return NotFound(new { message = "No learning slides found for the specified 3D model." });
            //}

            return Ok(slide.FirstOrDefault());
        }

        [HttpGet("GetSlideInfoBySlideTitle/{SlideTitle}")]
        public async Task<IActionResult> GetSlideInfoBySlideTitle(string SlideTitle)
        {
            var slide = await _context.AppLearningSlides
                .Where(s => s.Title.Trim().ToLower() == SlideTitle.Trim().ToLower())
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

            //if (slides == null || slides.Count == 0)
            //{
            //    return NotFound(new { message = "No learning slides found for the specified 3D model." });
            //}

            return Ok(slide.FirstOrDefault());
        }


        [HttpGet("GetSlidesByGameLevelName/{GameLevelName}")]
        public async Task<IActionResult> GetSlidesByGameLevelName(string GameLevelName)
        {
            var slides = await _context.AppLearningSlides
                .Where(s => s.AppGameLevel.Name.ToLower().Trim() == GameLevelName.ToLower().Trim())
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

            //if (slides == null || slides.Count == 0)
            //{
            //    return NotFound(new { message = "No learning slides found for the specified 3D model." });
            //}

            return Ok(slides);
        }

    }
}
