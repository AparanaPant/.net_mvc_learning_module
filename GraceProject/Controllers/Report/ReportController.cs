using GraceProject.Models;
using GraceProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GraceProject.Controllers.Report
{
    public class SearchModel
    {
        public string Keyword { get; set; }

        public int? Count { get; set; }
    }

    [Route("Report/Report")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly GraceDbContext _context;
        private readonly ILogger<ReportController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;


        public ReportController(GraceDbContext context, ILogger<ReportController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            //_logger = logger;
            //this._userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Report()
        {
            return View("~/Views/Report/Index.cshtml");
        }
        [HttpPost]
        public ActionResult Search(Report.ReportController model)
        {
            if (ModelState.IsValid)
            {
                // Perform search logic here and pass the result to the view
                return View("Index", model);
            }
            return View("Index", model);
        }



        //[HttpPost]
        //[Route("GetStudentList")]
        ////public IActionResult GetStudentList([FromBody] string keyword1)
        //public IActionResult GetStudentList(SearchModel searchData)
        //{
        //    try
        //    {
        //        string keyword = searchData.Keyword;
        //        int? count = searchData.Count;
        //        var Students = _context.Student
        //            .Where(s => s.StudentUser.FirstName.Contains(keyword) ||
        //                s.StudentUser.LastName.Contains(keyword) ||
        //                (s.StudentUser.UserName != null && s.StudentUser.UserName.Contains(keyword)))
        //            .Take(count != null ? count.Value : 100)
        //            .Select(s=> new {
        //                name = s.StudentUser.FirstName + " " + s.StudentUser.LastName + " (" + s.StudentUser.UserName + ") ",
        //                id = s.StudentUser.Id
        //            })
        //            .ToList();
                
        //        return Ok(Students);
                
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Error on Fetching Student Info: " + ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("GetTeacherList")]
        //public IActionResult GetTeacherList(SearchModel searchData)
        //{
        //    try
        //    {
        //        string keyword = searchData.Keyword;
        //        int? count = searchData.Count;
        //        var Teachers = _context.Educator
        //            .Where(e => e.EducatorUser.FirstName.Contains(keyword) || 
        //            e.EducatorUser.LastName.Contains(keyword) || 
        //            (e.EducatorUser.UserName != null && e.EducatorUser.UserName.Contains(keyword)))
        //            .Take(count != null ? count.Value : 100)
        //            .Select(s => new {
        //                Name = s.EducatorUser.FirstName + " " + s.EducatorUser.LastName + "(" + s.EducatorUser.UserName + ")",
        //                id = s.EducatorUser.Id
        //            })
        //            .ToList();
        //        return Ok(Teachers);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Error on Fetching Teacher Info: "+ ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("GetSchoolList")]
        public IActionResult GetSchoolList(SearchModel searchData)
        {
            try
            {
                string keyword = searchData.Keyword;
                int? count = searchData.Count;
                var Schools = _context.SchoolInfo
                    .Where(s => s.SchoolName.Contains(keyword))
                    .Take(count != null ? count.Value : 100)
                    .Select(s => new {
                        Name = s.SchoolName,
                        id = s.SchoolID
                    })
                    .ToList();
                return Ok(Schools);
            }
            catch (Exception ex)
            {
                return BadRequest("Error on Fetching School Info: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCoursesByStudent")]
        public IActionResult GetCoursesByStudent([FromBody] string dataId)
        {
            try
            {
                var CoursesByStudent = _context.Enrollment
                    .Where(e => e.StudentUser.Id == dataId)
                    .Select(e => new { 
                        name = e.Course.Title,
                        id = e.Course.CourseID
                    })
                    .ToList();

                return Ok(CoursesByStudent);
            }
            catch (Exception ex)
            {
                return BadRequest("Error on Fetching Corses By StudentID");
            }
        }

        [HttpPost]
        [Route("GetCoursesByEducator")]
        public IActionResult GetCoursesByEducator([FromBody] string dataId)
        {
            try
            {
                var CoursesByEducator = _context.Course
                    .Where(c => c.EducatorUserID == dataId)
                    .Select(c => new { 
                        name = c.Title,
                        id = c.CourseID
                    })
                    .ToList();

                return Ok(CoursesByEducator);
            }
            catch (Exception ex)
            {
                return BadRequest("Error on Fetching Corses By EducatorID");
            }
        }

        [HttpPost]
        [Route("GetCoursesBySchool")]
        public IActionResult GetCoursesBySchool([FromBody] int dataId)
        {

            try
            {
                var CoursesBySchool = _context.Course
                    .Where(c => c.SchoolID == dataId)
                    .Select(c => new { 
                        name = c.Title,
                        id = c.CourseID
                    })
                    .ToList();

                return Ok(CoursesBySchool);
            }
            catch (Exception ex)
            {
                return BadRequest("Error on Fetching Corses By SchoolID");
            }
        }
    }
}
