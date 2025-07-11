using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraceProject.Data;
using GraceProject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GraceProject.Controllers.Api;

namespace GraceProject.Controllers.Admin
{
    [Route("Admin/Slides")]
    [ApiController]

    public class SlidesController : Controller
    {
        private readonly GraceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SlidesController(GraceDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Modules
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var graceDbContext = _context.Module.Include(m => m.ApplicationUser);
            return View("~/views/Admin/Modules/Index.cshtml", await graceDbContext.ToListAsync());
        }



        // GET: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SlideInfo")]
        public IActionResult SlideInfo()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return PartialView("~/Views/Admin/Slides/SlideInfo.cshtml");
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SlideInfo")]
        public async Task<IActionResult> SlideInfo(Slide SlideInfo)
        {
            return PartialView(SlideInfo);
        }



        [HttpPost]
        [Route("AddOrEditSlideInfo")]
        public IActionResult AddOrEditSlideInfo([FromBody] string jsonData)
        {

            try
            {
                // Convert JSON string to object
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);

                // Extract values from the object
                var Action = jsonObject.Action;
                var SlideTitle = jsonObject.SlideTitle;
                int SlideId = -1;
                try
                {
                    SlideId = Convert.ToInt32(jsonObject.SlideId);
                }
                catch (Exception)
                {
                }
                var Order = jsonObject.Order;
                var DisplayThumbnailSlide = jsonObject.DisplayThumbnailSlide;
                var TumbnailSlideTitle = jsonObject.TumbnailSlideTitle;
                var TumbnailSlideIcon = jsonObject.TumbnailSlideIcon;
                var SlideType = jsonObject.SlideType;
                var ModuleId = jsonObject.ModuleId;
                var DisplayTitle = jsonObject.DisplayTitle;
                var ForwardButton = jsonObject.ForwardButton;
                var BackwardButton = jsonObject.BackwardButton;


                var rowOrColumn1Parameters = jsonObject.RowOrColumn1Parameters != null ? JObject.Parse(jsonObject.RowOrColumn1Parameters.Value) : null;
                var rowOrColumn2Parameters = jsonObject.RowOrColumn2Parameters != null ? JObject.Parse(jsonObject.RowOrColumn2Parameters.Value) : null;
                var rowOrColumn3Parameters = jsonObject.RowOrColumn3Parameters != null ? JObject.Parse(jsonObject.RowOrColumn3Parameters.Value) : null;
                var rowOrColumn4Parameters = jsonObject.RowOrColumn4Parameters != null ? JObject.Parse(jsonObject.RowOrColumn4Parameters.Value) : null;


                var UserGUID = HttpContext.User.Claims.FirstOrDefault().Value;

                if (rowOrColumn1Parameters == null && rowOrColumn2Parameters == null && rowOrColumn3Parameters == null && rowOrColumn4Parameters == null)
                {
                    return BadRequest("Please enter data for at least one row/column.");
                }

                int NumSlideSections = 0;
                if (rowOrColumn1Parameters != null)
                    NumSlideSections++;
                if (rowOrColumn2Parameters != null)
                    NumSlideSections++;
                if (rowOrColumn3Parameters != null)
                    NumSlideSections++;
                if (rowOrColumn4Parameters != null)
                    NumSlideSections++;


                if (UserGUID == null)
                {
                    return BadRequest("Please login to the system and try again later.");
                }

                if (ModelState.IsValid)
                {

                    //.................Slide Info.................
                    var slide = Action == "add" ? new Slide() : _context.Slide.Include(s=>s.SlideSections).FirstOrDefault(s => s.Id == SlideId);
                    slide.Title = SlideTitle;
                    slide.ModuleId = ModuleId;
                    slide.UserId = UserGUID;
                    slide.SavedDateTime = DateTime.Now;
                    //Thumbnail Info
                    slide.DisplayThumbnailSlide = DisplayThumbnailSlide;
                    slide.ShortTitle = TumbnailSlideTitle;
                    slide.ThumbIcon = TumbnailSlideIcon;
                    slide.DisplayTitle = DisplayTitle;
                    slide.SlideSectionsType = SlideType;
                    slide.NumSlideSections = NumSlideSections > 0 ? NumSlideSections : 0;
                    slide.ForwardButton = ForwardButton;
                    slide.BackwardButton = BackwardButton;
                    slide.SlideOrder = Order;
                    slide.SlideSections=new List<SlideSection>();

                    //.................Slide Section Info.................

                    //delete current sections and then add new sections
                    /*if (Action == "update")
                    {
                        var slidesections = _context.SlideSection.Where(ss => ss.SlideId == SlideId);
                        _context.SlideSection.RemoveRange(slidesections);
                        _context.SaveChanges();
                    }*/

                    var slideSections = new List<SlideSection>();

                    //section 1
                    if (rowOrColumn1Parameters != null && ((Newtonsoft.Json.Linq.JContainer)rowOrColumn1Parameters).Count > 0)
                    {
                        slideSections.Add(new SlideSection
                        {
                            SlideId = slide.Id,
                            Title = "",
                            Description = rowOrColumn1Parameters.ControllerName == "Text" ? rowOrColumn1Parameters.Content : "",
                            ControllerName = rowOrColumn1Parameters.ControllerName == "Chart" ? "PieChart" : rowOrColumn1Parameters.ControllerName,
                            ControllerParameters = rowOrColumn1Parameters.ControllerName == "Text" ? "" : jsonObject.RowOrColumn1Parameters.Value,
                            TextToVoice = rowOrColumn1Parameters.TextToVoice != null ? rowOrColumn1Parameters.TextToVoice : false,
                            SlideSectionOrder = 1,
                            UserId = UserGUID,
                            SavedDateTime = System.DateTime.Now

                        });
                    }

                    //section 2
                    if (rowOrColumn2Parameters != null && ((Newtonsoft.Json.Linq.JContainer)rowOrColumn2Parameters).Count > 0)
                    {
                        slideSections.Add(new SlideSection
                        {
                            SlideId = slide.Id,
                            Title = "",
                            Description = rowOrColumn2Parameters.ControllerName == "Text" ? rowOrColumn2Parameters.Content : "",
                            ControllerName = rowOrColumn2Parameters.ControllerName == "Chart" ? "PieChart" : rowOrColumn2Parameters.ControllerName,
                            ControllerParameters = rowOrColumn2Parameters.ControllerName == "Text" ? "" : jsonObject.RowOrColumn2Parameters.Value,
                            TextToVoice = rowOrColumn2Parameters.TextToVoice != null ? rowOrColumn2Parameters.TextToVoice : false,
                            SlideSectionOrder = 2,
                            UserId = UserGUID,
                            SavedDateTime = System.DateTime.Now

                        });
                    }

                    //section 3
                    if (rowOrColumn3Parameters != null && ((Newtonsoft.Json.Linq.JContainer)rowOrColumn3Parameters).Count > 0)
                    {
                        slideSections.Add(new SlideSection
                        {
                            SlideId = slide.Id,
                            Title = "",
                            Description = rowOrColumn3Parameters.ControllerName == "Text" ? rowOrColumn3Parameters.Content : "",
                            ControllerName = rowOrColumn3Parameters.ControllerName == "Chart" ? "PieChart" : rowOrColumn3Parameters.ControllerName,
                            ControllerParameters = rowOrColumn3Parameters.ControllerName == "Text" ? "" : jsonObject.RowOrColumn3Parameters.Value,
                            TextToVoice = rowOrColumn3Parameters.TextToVoice!=null? rowOrColumn3Parameters.TextToVoice:false,
                            SlideSectionOrder = 3,
                            UserId = UserGUID,
                            SavedDateTime = System.DateTime.Now
                        });
                    }

                    //section 4
                    if (rowOrColumn4Parameters != null && ((Newtonsoft.Json.Linq.JContainer)rowOrColumn4Parameters).Count > 0)
                    {
                        slideSections.Add(new SlideSection
                        {
                            SlideId = slide.Id,
                            Title = "",
                            Description = rowOrColumn4Parameters.ControllerName == "Text" ? rowOrColumn4Parameters.Content : "",
                            ControllerName = rowOrColumn4Parameters.ControllerName == "Chart" ? "PieChart" : rowOrColumn4Parameters.ControllerName,
                            ControllerParameters = rowOrColumn4Parameters.ControllerName == "Text" ? "" : jsonObject.RowOrColumn4Parameters.Value,
                            TextToVoice = rowOrColumn4Parameters.TextToVoice != null ? rowOrColumn4Parameters.TextToVoice : false,
                            SlideSectionOrder = 4,
                            UserId = UserGUID,
                            SavedDateTime = System.DateTime.Now

                        });
                    }


                    slide.SlideSections=slideSections;
                    if (Action == "add")
                        _context.Slide.Add(slide);
                    _context.SaveChanges();

                    return Ok("Data saved successfully");
                }

                return BadRequest(); // Return 400 Bad Request if the model state is not valid
            }
            catch (Exception EX)
            {
                var errorDetails = new
                {
                    message = EX.Message+"////"+ EX.InnerException,
                    exception = EX.GetType().Name,
                    stackTrace = EX.StackTrace
                };

                return BadRequest(errorDetails); // Return 400 Bad Request if the model state is not valid
            }
        }

        [HttpPost]
        [Route("DeleteSlideInfo")]
        public IActionResult DeleteSlideInfo([FromBody] int SlideId)
        {
            try
            {
                var slide = _context.Slide.FirstOrDefault(s => s.Id == SlideId);
                _context.Slide.Remove(slide);
                _context.SaveChangesAsync();
                return Ok("Slide deleted successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Error on deleting slide");

            }
        }

        [HttpPost]
        [Route("GetSlideList")]
        public IActionResult GetSlideList([FromBody] int ModuleId)
        {
            try
            {
                var Slides = _context.Slide.Where(s=>s.ModuleId== ModuleId);
                return Ok(Slides);
            }
            catch (Exception ex)
            {
                return BadRequest("Error on deleting slide");
            }
        }
        [HttpPost]
        [Route("GetSlideInfo")]
        public IActionResult GetSlideInfo([FromBody] int SlideId)
        {
            try
            {
                var Slide = _context.Slide.Include(s => s.SlideSections).FirstOrDefault(s => s.Id == SlideId);
                return Ok(Slide);
            }
            catch (Exception ex)
            {
                return BadRequest("Error on deleting slide");
            }
        }
    }
}
