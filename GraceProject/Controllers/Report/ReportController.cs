using GraceProject.Models;
using GraceProject.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GraceProject.Models;
using GraceProject.ViewModels;
using Newtonsoft.Json.Linq;


namespace GraceProject.Controllers.Report
{
    [Route("Report/Report")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly GraceDbContext _context;

        public ReportController(GraceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View("~/Views/Report/AdminReport.cshtml");
        }

        // Get Student List
        [HttpPost("GetStudentList")]
        public async Task<IActionResult> GetStudentList([FromBody] SearchModel searchData)
        {
            // Get the Role ID for 'Student'
            var studentRoleId = await _context.Roles
                .Where(r => r.Name == "Student")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // If no student role is found, return empty list
            if (string.IsNullOrEmpty(studentRoleId))
            {
                return Ok(new List<object>());
            }

            // Fetch students by Role ID and match by name or email
            var students = await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == studentRoleId))
                .Where(u => u.FirstName.Contains(searchData.Keyword) ||
                            u.LastName.Contains(searchData.Keyword) ||
                            u.Email.Contains(searchData.Keyword))  // <-- Match by Email too
                .Select(u => new
                {
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,  // Include Email in the result
                    id = u.Id
                })
                .ToListAsync();

            // Log the result count for debugging
            Console.WriteLine("Students Found: " + students.Count);

            return Ok(students);
        }

        // Courses by student name
        [HttpPost("GetStudentCourses")]
        public async Task<IActionResult> GetStudentCourses([FromBody] SearchModel searchData)
        {
            var name = searchData.Keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string firstName = name[0];
            string lastName = name[1];
            //string ID = searchData.Keyword;

            // Use LINQ to join the tables and retrieve courses.
            var courses = await (from user in _context.Users
                                 where user.FirstName == firstName && user.LastName == lastName
                                 join studentSession in _context.StudentSessions on user.Id equals studentSession.StudentID
                                 join session in _context.Session on studentSession.SessionID equals session.SessionID
                                 join course in _context.Course on session.CourseID equals course.CourseID
                                 select new
                                 {
                                     name = course.Title + " (" + (course.Credits ?? 0) + " Credits)",
                                     id = course.CourseID
                                 })
                         .Distinct()
                         .ToListAsync();

            // Log the result count for debugging
            //Console.WriteLine("Courses Found: " + courses.Count);

            return Ok(courses);
        }

        // Courses by educator name
        [HttpPost("GetEducatorCourses")]
        public async Task<IActionResult> GetEducatorCourses([FromBody] SearchModel searchData)
        {
            var name = searchData.Keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string firstName = name[0];
            string lastName = name[1];

            // Use LINQ to join the tables and retrieve courses.
            var courses = await (from user in _context.Users
                                 where user.FirstName == firstName && user.LastName == lastName
                                 join educatorSession in _context.EducatorSession on user.Id equals educatorSession.EducatorID
                                 join session in _context.Session on educatorSession.SessionID equals session.SessionID
                                 join course in _context.Course on session.CourseID equals course.CourseID
                                 select new
                                 {
                                     name = course.Title + " (" + (course.Credits ?? 0) + " Credits)",
                                     id = course.CourseID
                                 })
                         .Distinct()
                         .ToListAsync();

            // Log the result count for debugging
            //Console.WriteLine("Courses Found: " + courses.Count);

            return Ok(courses);
        }

        // Get Course List
        [HttpPost("GetCourseList")]
        public async Task<IActionResult> GetCourseList([FromBody] SearchModel searchData)
        {



            // Fetch all courses matching the keyword (if provided)
            var courses = await _context.Course
                .Where(c => string.IsNullOrEmpty(searchData.Keyword) || c.Title.Contains(searchData.Keyword))
                .Select(c => new
                {
                    name = c.Title + " (" + (c.Credits ?? 0) + " Credits)",  // Show title with credits in brackets
                    id = c.CourseID
                })
                .ToListAsync();

            // Log the result count for debugging
            Console.WriteLine("Courses Found: " + courses.Count);

            return Ok(courses);
        }

        // Get Sessions By Student
        [HttpPost("GetSessionsByStudent")]
        public IActionResult GetSessionsByStudent([FromBody] IdModel idModel)
        {
            var sessions = _context.StudentSessions
                .Where(ss => ss.StudentID == idModel.Id)
                .Select(ss => new
                {
                    name = ss.Session.Course.Title + " - " +
                           _context.Users
                               .Where(u => ss.Session.EducatorSessions.Any(es => es.EducatorID == u.Id))
                               .Select(u => u.FirstName + " " + u.LastName)
                               .FirstOrDefault(),
                    id = ss.SessionID
                })
                .ToList();

            return Ok(sessions);
        }

        // Get Sessions By Course
        [HttpPost("GetSessionsByCourse")]
        public IActionResult GetSessionsByCourse([FromBody] CourseIdModel model)
        {
            var sessions = _context.Session
                .Where(s => s.CourseID == model.Id) // Fix: Use 'model.Id' instead of 'idModel.Id'
                .Select(s => new
                {
                    name = _context.Users
                        .Where(u => _context.EducatorSession.Any(es => es.SessionID == s.SessionID && es.EducatorID == u.Id))
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Instructor Not Assigned",
                    id = s.SessionID
                })
                .ToList();

            return Ok(sessions);
        }

        [HttpPost("GetEducatorList")]
        public async Task<IActionResult> GetEducatorList([FromBody] SearchModel searchData)
        {
            // Fetch educator role ID
            var educatorRoleId = await _context.Roles
                .Where(r => r.Name == "Educator")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // If educator role is not found, return empty list
            if (string.IsNullOrEmpty(educatorRoleId))
            {
                return Ok(new List<object>());
            }

            // Fetch educators based on role ID and search keyword
            var educators = await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == educatorRoleId))
                .Where(u => u.FirstName.Contains(searchData.Keyword) || u.LastName.Contains(searchData.Keyword) || u.Email.Contains(searchData.Keyword))
                .Select(u => new
                {
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    id = u.Id
                })
                .ToListAsync();

            // Log result for debugging
            Console.WriteLine("Educators Found: " + educators.Count);

            return Ok(educators);
        }


        [HttpPost("GetSessionsByEducator")]
        public IActionResult GetSessionsByEducator([FromBody] IdModel idModel)
        {
            var sessions = _context.EducatorSession
                .Where(es => es.EducatorID == idModel.Id)
                .Select(es => new { name = es.Session.Course.Title, id = es.SessionID })
                .ToList();

            return Ok(sessions);
        }

        [HttpPost("GetStudentGrades")]
        public async Task<IActionResult> GetStudentGrades([FromBody] StudentSessionModel model)
        {
            // Validate input
            if (model == null || string.IsNullOrEmpty(model.Keyword) || string.IsNullOrEmpty(model.CourseID))
            {
                return BadRequest("Invalid request parameters.");
            }

            // Use the common date filter method
            var dateRange = GetDateRange(model.DateFilter, model.StartDate, model.EndDate);
            if (dateRange == null)
            {
                return BadRequest("Invalid or missing date range.");
            }

            DateTime startDate = dateRange.Value.startDate;
            DateTime endDate = dateRange.Value.endDate;

            // Fetch quiz results within the selected date range
            var quizResults = await _context.UserQuizzes
                .Where(uq => uq.UserId == model.Keyword
                             && uq.Quiz.CourseID == model.CourseID
                             && uq.Quiz.CreatedAt >= startDate
                             && uq.Quiz.CreatedAt <= endDate) // Apply date filter
                .Select(uq => new
                {
                    QuizId = uq.QuizId,
                    QuizTitle = uq.Quiz.Title,
                    Score = uq.Score ?? 0,
                    FullMarks = uq.Quiz.TotalScore ?? 0,
                    Date = uq.StartedAt
                })
                .ToListAsync();

            var unqRes = quizResults.GroupBy(q => q.QuizId).Select(group => group.OrderByDescending(q => q.Date).FirstOrDefault()).ToList();

            int totalScore = unqRes.Sum(q => q.Score);
            int totalFullMarks = unqRes.Sum(q => q.FullMarks);
            int totalQuizzesAttempted = unqRes.Count;

            // Calculate percentage
            double totalPercentage = totalFullMarks > 0
                ? ((double)totalScore / totalFullMarks) * 100
                : 0;

            // Calculate average score per quiz
            double averageScorePerQuiz = totalQuizzesAttempted > 0
                ? (double)totalScore / totalQuizzesAttempted
                : 0;

            var result = new
            {
                QuizResults = quizResults,
                TotalScore = totalScore,
                TotalFullMarks = totalFullMarks,
                TotalPercentage = Math.Round(totalPercentage, 2), // Round to 2 decimal places
                AverageScorePerQuiz = Math.Round(averageScorePerQuiz, 2),
                TotalQuizzesAttempted = totalQuizzesAttempted,
                PassStatus = totalPercentage >= 60 ? "Passed" : "Failed" // Example: Pass if ≥ 50%
            };

            return Ok(result);
        }

        [HttpPost("GetSchoolList")]
        public async Task<IActionResult> GetSchoolList([FromBody] SearchModel searchData)
        {
            var schools = await _context.Schools
                .Where(s => s.SchoolName.Contains(searchData.Keyword))
                .Select(s => new
                {
                    name = s.SchoolName,
                    id = s.SchoolID
                })
                .ToListAsync();

            return Ok(schools);
        }

        [HttpPost("GetSchoolGrades")]
        public async Task<IActionResult> GetSchoolGrades([FromBody] SchoolCourseRequestModel model)
        {
            int schoolId = model.Id;
            string? courseId = model.CourseID; // Course ID is now properly accessible

            var studentIds = await _context.UserSchools
                .Where(us => us.SchoolID == schoolId)
                .Select(us => us.UserID)
                .ToListAsync();

            if (!studentIds.Any())
            {
                return Ok(new { message = "No students found for this school." });
            }

            // Get date range
            var dateRange = GetDateRange(model.DateFilter, model.StartDate, model.EndDate);
            if (dateRange == null)
            {
                return BadRequest("Invalid or missing date range.");
            }

            DateTime startDate = dateRange.Value.startDate;
            DateTime endDate = dateRange.Value.endDate;

            // Fetch quiz results, filtering by Course ID if provided
            var quizResults = await _context.UserQuizzes
                .Where(uq => studentIds.Contains(uq.UserId)
                    && uq.Quiz.CreatedAt >= startDate
                    && uq.Quiz.CreatedAt <= endDate
                    && (courseId == null || uq.Quiz.CourseID == courseId))
                .Select(uq => new
                {
                    StudentId = uq.UserId,
                    StudentName = _context.Users
                        .Where(u => u.Id == uq.UserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault(),
                    CourseId = uq.Quiz.CourseID,
                    CourseTitle = uq.Quiz.Course.Title,
                    QuizTitle = uq.Quiz.Title,
                    Score = uq.Score ?? 0,
                    FullMarks = uq.Quiz.TotalScore ?? 0,
                    QuizDate = uq.Quiz.CreatedAt
                })
                .ToListAsync();

            var groupedResults = quizResults
                .Where(q => !string.IsNullOrEmpty(q.CourseTitle) && q.CourseTitle != "Unknown Course")
                .GroupBy(q => q.CourseTitle)
                .Select(courseGroup => new
                {
                    CourseId = courseGroup.First().CourseId,
                    CourseTitle = courseGroup.Key,
                    TotalQuizzes = courseGroup.Count(),
                    TotalStudents = courseGroup.Select(q => q.StudentId).Distinct().Count(),
                    AverageScore = courseGroup.Average(q => q.Score),
                    TotalScore = courseGroup.Sum(q => q.Score),
                    TotalFullMarks = courseGroup.Sum(q => q.FullMarks),
                    PassPercentage = (courseGroup.Count(q => q.Score >= q.FullMarks * 0.5) * 100.0) / courseGroup.Count(),
                    Students = courseGroup
                        .GroupBy(s => s.StudentId)
                        .Select(studentGroup => new
                        {
                            StudentId = studentGroup.Key,
                            StudentName = studentGroup.First().StudentName,
                            Quizzes = studentGroup.Select(q => new
                            {
                                QuizTitle = q.QuizTitle,
                                Score = q.Score,
                                FullMarks = q.FullMarks,
                                Percentage = (q.FullMarks > 0) ? Math.Round((q.Score / (double)q.FullMarks) * 100, 2) : 0,
                                QuizDate = q.QuizDate
                            }).OrderBy(q => q.QuizDate).ToList(),
                            TotalScore = studentGroup.Sum(q => q.Score),
                            TotalFullMarks = studentGroup.Sum(q => q.FullMarks),
                            OverallPercentage = (studentGroup.Sum(q => q.Score) / (double)studentGroup.Sum(q => q.FullMarks)) * 100
                        }).OrderByDescending(s => s.OverallPercentage).ToList()
                })
                .ToList();

            if (!groupedResults.Any())
            {
                return Ok(new { message = "No valid courses found in the selected date range." });
            }

            var result = new
            {
                SchoolId = schoolId,
                SchoolName = _context.Schools.Where(s => s.SchoolID == schoolId).Select(s => s.SchoolName).FirstOrDefault(),
                SelectedCourseId = courseId,
                SelectedCourseTitle = courseId != null
                ? _context.Course.Where(c => c.CourseID == courseId).Select(c => c.Title).FirstOrDefault()
                : "All Courses",
                TotalStudents = studentIds.Count,
                TotalQuizzes = quizResults.Count,
                OverallAverageScore = quizResults.Any() ? quizResults.Average(q => q.Score) : 0,
                OverallPassPercentage = quizResults.Any()
                    ? (quizResults.Count(q => q.Score >= q.FullMarks * 0.5) * 100.0) / quizResults.Count()
                    : 0,
                Courses = groupedResults
            };

            return Ok(result);
        }


        [HttpPost("GetCourseGrades")]
        public async Task<IActionResult> GetCourseGrades([FromBody] IdModel model)
        {
            if (_context.Course == null || _context.Session == null)
            {
                return NotFound("Courses or Sessions table is not available.");
            }

            // Get session information
            var session = await _context.Session.FirstOrDefaultAsync(s => s.SessionID == Convert.ToInt32(model.Id));
            if (session == null)
            {
                return NotFound("Session not found.");
            }

            // Get course information
            var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseID == session.CourseID);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // Use the common date filter method
            var dateRange = GetDateRange(model.DateFilter, model.StartDate, model.EndDate);
            if (dateRange == null)
            {
                return BadRequest("Invalid or missing date range.");
            }

            DateTime startDate = dateRange.Value.startDate;
            DateTime endDate = dateRange.Value.endDate;

            // Fetch student sessions
            var studentSessions = await _context.StudentSessions
                .Where(ss => ss.SessionID == session.SessionID)
                .ToListAsync();

            // Fetch users
            var users = await _context.Users.ToListAsync();

            // Apply date filtering to quizzes
            var quizzes = await _context.Quizzes
                .Where(q => (q.CourseID == course.CourseID || q.SessionID == session.SessionID)
                            && q.CreatedAt >= startDate && q.CreatedAt <= endDate)
                .ToListAsync();

            // Apply date filtering to user quizzes
            var userQuizzes = await _context.UserQuizzes
                .Where(uq => uq.Quiz.CreatedAt >= startDate && uq.Quiz.CreatedAt <= endDate)
                .ToListAsync();

            // Process in-memory using AsEnumerable()
            var students = studentSessions
                .AsEnumerable()
                .Select(ss => new StudentQuizResultViewModel
                {
                    StudentId = ss.StudentID,
                    StudentName = users
                        .Where(u => u.Id.ToString() == ss.StudentID)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Unknown Student",
                    Quizzes = quizzes
                        .Select(q => new EducatorQuizResultViewModel
                        {
                            QuizTitle = q.Title,
                            TotalScore = q.TotalScore ?? 0,
                            ObtainedScore = userQuizzes
                                .Where(uq => uq.UserId == ss.StudentID && uq.QuizId == q.QuizId)
                                .Sum(uq => uq.Score) ?? 0
                        }).ToList()
                }).ToList();

            // Calculate total and grade for each student
            foreach (var student in students)
            {
                student.TotalScore = student.Quizzes.Sum(q => q.TotalScore);
                student.ObtainedScore = student.Quizzes.Sum(q => q.ObtainedScore);
                student.Percentage = student.TotalScore > 0 ? (student.ObtainedScore / (double)student.TotalScore) * 100 : 0;
                student.Grade = GetGrade(student.Percentage);
            }

            // Prepare the result model
            var result = new TeacherGradebookViewModel
            {
                CourseTitle = course.Title ?? "Unknown Course",
                SessionTitle = session.IsActive ? "Active Session" : "Inactive Session",
                Students = students
            };

            return Ok(result);
        }

        [HttpPost("GetModulesByCourse")]
        public async Task<IActionResult> GetModulesByCourse([FromBody] CourseIdModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return BadRequest("Course Id is required.");
            }

            // Query modules for the given course
            var modules = await _context.Module
                .Where(m => m.CourseId == model.Id)
                .Select(m => new
                {
                    id = m.Id,
                    moduleName = m.ModuleName
                })
                .ToListAsync();

            return Ok(modules);
        }

        [HttpPost("GetStudentSlideTime")]
        public async Task<IActionResult> GetStudentSlideTime([FromBody] StudentSlideTimeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StudentID)
             || string.IsNullOrWhiteSpace(request.CourseID)
             || request.ModuleID == 0)
                return BadRequest("Invalid request parameters.");

            var dateRange = GetDateRange(request.DateFilter, request.StartDate, request.EndDate);
            if (dateRange == null)
                return BadRequest("Invalid or missing date range.");

            var (start, end) = dateRange.Value;

            var perSlide = await _context.SlideReadTracking
                .Include(srt => srt.Slide)
                .Where(srt =>
                    srt.UserId == request.StudentID &&
                    srt.Slide.ModuleId == request.ModuleID &&
                    srt.ReadDateTime >= start &&
                    srt.ReadDateTime <= end)
                .GroupBy(srt => new { srt.SlideId, srt.Slide.Title })
                .Select(g => new {
                    SlideId = g.Key.SlideId,
                    SlideTitle = g.Key.Title,
                    TimeSpent = g.Sum(x => x.DurationSeconds)
                })
                .ToListAsync();

            return Ok(new
            {
                TotalTimeSpent = perSlide.Sum(x => x.TimeSpent),
                Slides = perSlide
            });
        }

        //[HttpPost("GetStudentSlidesViewedCount")]
        //public async Task<IActionResult> GetStudentSlidesViewedCount([FromBody] StudentSlideTimeRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.StudentID)
        //     || string.IsNullOrWhiteSpace(request.CourseID)
        //     || request.ModuleID == 0)
        //        return BadRequest("Invalid request parameters.");

        //    var dateRange = GetDateRange(request.DateFilter, request.StartDate, request.EndDate);
        //    if (dateRange == null)
        //        return BadRequest("Invalid or missing date range.");

        //    var (start, end) = dateRange.Value;

        //    var slidesViewedCount = await _context.SlideReadTracking
        //        .Include(srt => srt.Slide)
        //        .Where(srt =>
        //            srt.UserId == request.StudentID &&
        //            srt.Slide.ModuleId == request.ModuleID &&
        //            srt.ReadDateTime >= start &&
        //            srt.ReadDateTime <= end)
        //        .Select(srt => srt.SlideId)
        //        .Distinct()
        //        .CountAsync();

        //    return Ok(new { SlidesViewedCount = slidesViewedCount });
        //}

        [HttpPost("GetStudentSlidesViewedDetails")]
        public async Task<IActionResult> GetStudentSlidesViewedDetails([FromBody] StudentSlideTimeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StudentID)
             || string.IsNullOrWhiteSpace(request.CourseID)
             || request.ModuleID == 0)
                return BadRequest("Invalid request parameters.");

            var dateRange = GetDateRange(request.DateFilter, request.StartDate, request.EndDate);
            if (dateRange == null)
                return BadRequest("Invalid or missing date range.");

            var (start, end) = dateRange.Value;

            // Get all slides in the module
            int totalSlidesInModule = await _context.Slide
                .Where(s => s.ModuleId == request.ModuleID)
                .CountAsync();

            // Get the slides the student viewed
            var viewedSlides = await _context.SlideReadTracking
                .Include(srt => srt.Slide)
                .Where(srt =>
                    srt.UserId == request.StudentID &&
                    srt.Slide.ModuleId == request.ModuleID &&
                    srt.ReadDateTime >= start &&
                    srt.ReadDateTime <= end)
                .GroupBy(srt => new { srt.SlideId, srt.Slide.Title })
                .Select(g => new {
                    SlideId = g.Key.SlideId,
                    SlideTitle = g.Key.Title,
                    TimeSpent = g.Sum(x => x.DurationSeconds),
                    LastViewed = g.Max(x => x.ReadDateTime) // Get the latest time this slide was accessed
                })
                .ToListAsync();

            int slidesViewedCount = viewedSlides.Count;
            double percentageViewed = totalSlidesInModule > 0
                ? Math.Round((slidesViewedCount / (double)totalSlidesInModule) * 100, 2)
                : 0;

            var lastViewedSlide = viewedSlides.OrderByDescending(s => s.LastViewed).FirstOrDefault();

            return Ok(new
            {
                TotalSlidesInModule = totalSlidesInModule,
                SlidesViewedCount = slidesViewedCount,
                PercentageViewed = percentageViewed,
                ViewedSlides = viewedSlides,
                LastViewedSlide = lastViewedSlide
            });
        }



        [HttpPost("GetEducatorCourseGrades")]
        public async Task<IActionResult> GetEducatorCourseGrades([FromBody] EducatorCourseModel model)
        {
            if (_context.Course == null)
            {
                return NotFound("Courses table is not available.");
            }

            var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseID == model.CourseID);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // Get the date range using the common method
            var dateRange = GetDateRange(model.DateFilter, model.StartDate, model.EndDate);
            if (dateRange == null)
            {
                return BadRequest("Invalid or missing date range.");
            }

            DateTime startDate = dateRange.Value.startDate;
            DateTime endDate = dateRange.Value.endDate;

            // Get student sessions
            var studentSessions = await _context.StudentSessions
                .Where(ss => ss.Session.CourseID == model.CourseID)
                .ToListAsync();

            // Get all users (students)
            var users = await _context.Users.ToListAsync();

            // Get quizzes filtered by course and date range
            var quizzes = await _context.Quizzes
                .Where(q => q.CourseID == model.CourseID && q.CreatedAt >= startDate && q.CreatedAt <= endDate)
                .ToListAsync();

            // Get student quiz results
            var userQuizzes = await _context.UserQuizzes
                .Where(uq => uq.Quiz.CourseID == model.CourseID && uq.Quiz.CreatedAt >= startDate && uq.Quiz.CreatedAt <= endDate)
                .ToListAsync();

            // Process and group student results
            var students = studentSessions
                .Select(ss => new StudentQuizResultViewModel
                {
                    StudentId = ss.StudentID,
                    StudentName = users
                        .Where(u => u.Id.ToString() == ss.StudentID)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault() ?? "Unknown Student",
                    Quizzes = quizzes
                        .Select(q => new EducatorQuizResultViewModel
                        {
                            QuizTitle = q.Title,
                            TotalScore = q.TotalScore ?? 0,
                            ObtainedScore = userQuizzes
                            //    .Where(uq => uq.UserId == ss.StudentID && uq.QuizId == q.QuizId)
                            //.Sum(uq => uq.Score) ?? 0
                                .LastOrDefault(uq => uq.UserId == ss.StudentID && uq.QuizId == q.QuizId)?.Score ?? 0

                        }).ToList()
                }).ToList();

            // 🔹 Calculate total score, obtained score, percentage, and grade per student
            foreach (var student in students)
            {
                student.TotalScore = student.Quizzes.Sum(q => q.TotalScore);
                student.ObtainedScore = student.Quizzes.Sum(q => q.ObtainedScore);
                student.Percentage = student.TotalScore > 0 ? (student.ObtainedScore / (double)student.TotalScore) * 100 : 0;
                student.Grade = GetGrade(student.Percentage);
            }

            if (students.Count == 0)
            {
                return Ok(new { message = "No grades available for this course in the selected date range." });
            }

            var result = new TeacherGradebookViewModel
            {
                CourseTitle = course.Title ?? "Unknown Course",
                Students = students
            };

            return Ok(result);
        }

        // Helper method to determine grade
        private string GetGrade(double percentage)
        {
            if (percentage >= 90) return "A";
            else if (percentage >= 80) return "B";
            else if (percentage >= 70) return "C";
            else if (percentage >= 60) return "D";
            else return "F";
        }

        private (DateTime startDate, DateTime endDate)? GetDateRange(string dateFilter, DateTime? startDate, DateTime? endDate)
        {
            switch (dateFilter)
            {
                case "weekly":
                    return (DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
                case "monthly":
                    return (DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
                case "last3months":
                    return (DateTime.UtcNow.AddMonths(-3), DateTime.UtcNow);
                case "custom":
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        return (startDate.Value, endDate.Value);
                    }
                    return null;
                default:
                    return null;
            }
        }


    }

    // Models for AJAX
    public class SearchModel
    {
        public string Keyword { get; set; }
        string UserId { get; set; }
        string SearchType { get; set; }
    }
    public class StudentSlideTimeRequest
    {
        public string StudentID { get; set; }
        public string CourseID { get; set; }
        public int ModuleID { get; set; }
        public string DateFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class IdModel
    {
        public string Id { get; set; }

        public string DateFilter { get; set; } // "weekly", "monthly", "last3months", "custom"
        public DateTime? StartDate { get; set; } // For custom date range
        public DateTime? EndDate { get; set; } // For custom date range
    }

    public class CourseIdModel
    {
        public string Id { get; set; }
    }

}
