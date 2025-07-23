// Controllers/QuizController.cs
using GraceProject.Controllers.Api;
using GraceProject.Data;
using GraceProject.Models;
using GraceProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GraceProject.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly GraceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QuizController(GraceDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // List all quizzes
        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return View(quizzes);
        }

        [HttpGet]
        public IActionResult Create(string? courseId, int? sessionId, int? gameLevelId)
        {
            // Ensure that at least one identifier is provided
            if (string.IsNullOrEmpty(courseId) && !sessionId.HasValue && !gameLevelId.HasValue)
            {
                return BadRequest("A valid Course ID, Session ID, or Game Level ID is required.");
            }

            var model = new QuizViewModel
            {
                CourseID = courseId ?? string.Empty,
                SessionID = sessionId,
                GameLevelID = gameLevelId, 
                Questions = new List<QuestionViewModel>()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile ImageFile, int questionIndex)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var quizImagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "quiz-images");

                if (!Directory.Exists(quizImagesFolder))
                {
                    Directory.CreateDirectory(quizImagesFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(quizImagesFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                return Json(new
                {
                    imageUrl = "/quiz-images/" + uniqueFileName,
                    questionIndex = questionIndex // ✅ Return the question index to associate image with the correct question
                });
            }
            return BadRequest("Image upload failed.");
        }


        [HttpPost]
        public async Task<IActionResult> Create(QuizViewModel quizViewModel)
        {
            Console.WriteLine($"Quiz Title: {quizViewModel.Title}");
            foreach (var question in quizViewModel.Questions)
            {
                Console.WriteLine($"Question Text: {question.Text}");
                if (question.Type == "Fill in the Blank")
                {
                    foreach (var answer in question.FillInTheBlankAnswers ?? new List<string>())
                    {
                        Console.WriteLine($"Answer: {answer}");
                    }
                }
                else
                {
                    foreach (var option in question.Options ?? new List<OptionViewModel>())
                    {
                        Console.WriteLine($"Option Text: {option.Text}, IsCorrect: {option.IsCorrect}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var quiz = new Quiz
                {
                    Title = quizViewModel.Title,
                    Duration = quizViewModel.Duration,
                    CreatedAt = DateTime.Now,
                    CourseID = quizViewModel.CourseID,
                    SessionID = quizViewModel.SessionID,
                    GameLevelID = quizViewModel.GameLevelID,
                    TotalScore = quizViewModel.TotalScore,
                    DueDate = quizViewModel.DueDate,            
                    NoOfAttempts = quizViewModel.NoOfAttempts,  
                    Questions = quizViewModel.Questions.Select(q => new Question
                    {
                        Text = q.Text,
                        Type = q.Type,
                        Points = q.Points,
                        ImageUrl = q.ImageUrl,
                        Options = q.Type != "Fill in the Blank" && q.Options != null
                                  ? q.Options.Select(o => new Option
                                  {
                                      Text = o.Text,
                                      IsCorrect = o.IsCorrect
                                  }).ToList()
                                  : new List<Option>(),
                        FillInTheBlankAnswers = q.Type == "Fill in the Blank" && q.FillInTheBlankAnswers != null
                                                ? q.FillInTheBlankAnswers.Select(a => new FillInTheBlankAnswer
                                                {
                                                    Answer = a
                                                }).ToList()
                                                : new List<FillInTheBlankAnswer>()
                    }).ToList()
                };

                _context.Add(quiz);
                await _context.SaveChangesAsync();

                // Conditional redirects based on SessionID or CourseID
                if (quiz.GameLevelID.HasValue)
                {
                    return Redirect("/Quiz/SelectGameLevel");
                }
                else if (quiz.SessionID.HasValue)
                {
                    return Redirect($"/Educator/Quizzes/{quiz.SessionID}");
                }
                else if (!string.IsNullOrEmpty(quiz.CourseID))
                {
                    return Redirect($"/Admin/Courses/Quizzes/List/{quiz.CourseID}");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            Console.WriteLine("Model state is invalid.");
            foreach (var key in ModelState.Keys)
            {
                var value = ModelState[key];
                foreach (var error in value.Errors)
                {
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            return View(quizViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Take(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
            {
                return NotFound();
            }

            var quizViewModel = new QuizViewModel
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Duration = quiz.Duration,
                GameLevelID = quiz.GameLevelID,
                Questions = quiz.Questions.Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    Type = q.Type,
                    Text = q.Text,
                    Points = q.Points,
                    Options = q.Options.Select(o => new OptionViewModel
                    {
                        OptionId = o.OptionId,
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    }).ToList(),
                    ImageUrl = q.ImageUrl  // Ensure this property is set
                }).ToList()
            };

            return View(quizViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.FillInTheBlankAnswers)
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
            {
                return NotFound();
            }

            var quizViewModel = new QuizViewModel
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Duration = quiz.Duration,
                Questions = quiz.Questions.Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    Type = q.Type,
                    Text = q.Text,
                    Points = q.Points,
                    ImageUrl = q.ImageUrl,
                    Options = q.Options.Select(o => new OptionViewModel
                    {
                        OptionId = o.OptionId,
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    }).ToList(),
                    FillInTheBlankAnswers = q.FillInTheBlankAnswers.Select(a => a.Answer).ToList()
                }).ToList()
            };

            return View(quizViewModel);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(QuizViewModel model, string? userType)
        {
            if (ModelState.IsValid)
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.Options)
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.FillInTheBlankAnswers)
                    .FirstOrDefaultAsync(q => q.QuizId == model.QuizId);

                if (quiz == null)
                {
                    return NotFound();
                }

                // Update Quiz Details (existing code)
                if (!string.IsNullOrEmpty(model.Title))
                {
                    quiz.Title = model.Title;
                }
                if (model.Duration != 0)
                {
                    quiz.Duration = model.Duration;
                }

                // Process deletions for questions marked for deletion.
                // Assuming "QuestionsToDelete" is posted as multiple hidden inputs.
                var questionsToDelete = Request.Form["QuestionsToDelete"];
                if (!string.IsNullOrEmpty(questionsToDelete))
                {
                    foreach (var questionIdStr in questionsToDelete)
                    {
                        if (int.TryParse(questionIdStr, out int questionId))
                        {
                            var question = quiz.Questions.FirstOrDefault(q => q.QuestionId == questionId);
                            if (question != null)
                            {
                                // Remove the question from the context (and from the quiz collection if necessary)
                                _context.Questions.Remove(question);
                            }
                        }
                    }
                }

                // Update or add/update remaining questions
                foreach (var questionModel in model.Questions)
                {
                    var existingQuestion = quiz.Questions.FirstOrDefault(q => q.QuestionId == questionModel.QuestionId);
                    if (existingQuestion != null)
                    {
                        if (!string.IsNullOrEmpty(questionModel.Text))
                        {
                            existingQuestion.Text = questionModel.Text;
                        }
                        if (questionModel.Points != 0)
                        {
                            existingQuestion.Points = questionModel.Points;
                        }
                        if (!string.IsNullOrEmpty(questionModel.Type))
                        {
                            existingQuestion.Type = questionModel.Type;
                        }
                        if (!string.IsNullOrEmpty(questionModel.ImageUrl))
                        {
                            existingQuestion.ImageUrl = questionModel.ImageUrl;
                        }

                        if (questionModel.Type == "Multiple Choice" || questionModel.Type == "True/False")
                        {
                            existingQuestion.Options.Clear();
                            existingQuestion.Options = questionModel.Options.Select(o => new Option
                            {
                                OptionId = o.OptionId,
                                Text = o.Text,
                                IsCorrect = o.IsCorrect
                            }).ToList();
                        }

                        if (questionModel.Type == "Fill in the Blank")
                        {
                            existingQuestion.FillInTheBlankAnswers.Clear();
                            existingQuestion.FillInTheBlankAnswers = questionModel.FillInTheBlankAnswers.Select(a => new FillInTheBlankAnswer
                            {
                                Answer = a
                            }).ToList();
                        }
                    }
                    else
                    {
                        // This is a new question; add it to the quiz
                        var newQuestion = new Question
                        {
                            Text = questionModel.Text,
                            Points = questionModel.Points,
                            Type = questionModel.Type,
                            ImageUrl = questionModel.ImageUrl,
                            // Initialize collections if needed
                            Options = (questionModel.Type == "Multiple Choice" || questionModel.Type == "True/False")
                                ? questionModel.Options.Select(o => new Option
                                {
                                    OptionId = o.OptionId,
                                    Text = o.Text,
                                    IsCorrect = o.IsCorrect
                                }).ToList()
                                : new List<Option>(),
                            FillInTheBlankAnswers = (questionModel.Type == "Fill in the Blank")
                                ? questionModel.FillInTheBlankAnswers.Select(a => new FillInTheBlankAnswer
                                {
                                    Answer = a
                                }).ToList()
                                : new List<FillInTheBlankAnswer>()
                        };
                        quiz.Questions.Add(newQuestion);
                    }
                }

                await _context.SaveChangesAsync();

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return Redirect($"/Admin/Courses/Quizzes/List/{quiz.CourseID}");

                    if (await _userManager.IsInRoleAsync(user, "Educator"))
                        return Redirect($"/Educator/Quizzes/{quiz.CourseID}");
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([FromForm] QuizViewModel submitQuizViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
            var quiz = await _context.Quizzes
        .Include(q => q.Questions)
            .ThenInclude(q => q.Options)
        .Include(q => q.Questions)
            .ThenInclude(q => q.FillInTheBlankAnswers)
        .FirstOrDefaultAsync(q => q.QuizId == submitQuizViewModel.QuizId);

            if (quiz == null) return NotFound("Quiz not found");

            int totalScore = 0;
            List<UserAnswer> userAnswers = new List<UserAnswer>();

            var userQuiz = new UserQuiz
            {
                UserId = user.Id,
                QuizId = quiz.QuizId,
                CompletedAt = DateTime.Now,
                Score = 0
            };
            _context.UserQuizzes.Add(userQuiz);
            await _context.SaveChangesAsync();

            int userQuizId = userQuiz.UserQuizId;

            // Process each submitted answer
            foreach (var questionAnswer in submitQuizViewModel.QuestionAnswers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.QuestionId == questionAnswer.QuestionId);
                if (question == null) continue;

                bool isCorrect = false;
                int pointsAwarded = 0;

                if ((question.Type == "Multiple Choice" || question.Type == "True/False") && questionAnswer.SelectedOptionId.HasValue)
                {
                    var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == questionAnswer.SelectedOptionId.Value);
                    isCorrect = selectedOption?.IsCorrect ?? false;
                }
                if (question.Type == "Fill in the Blank" && !string.IsNullOrWhiteSpace(questionAnswer.FillInTheBlankResponse))
                {
                    var correctAnswers = question.FillInTheBlankAnswers?.Select(a => a.Answer).ToList() ?? new List<string>();

                    isCorrect = correctAnswers.Any(ans => ans.Equals(questionAnswer.FillInTheBlankResponse, StringComparison.OrdinalIgnoreCase));
                }

                if (isCorrect)
                {
                    pointsAwarded = question.Points;
                    totalScore += pointsAwarded;
                }

                userAnswers.Add(new UserAnswer
                {
                    UserQuizId = userQuizId,
                    QuestionId = question.QuestionId,
                    SelectedOptionId = question.Type == "Fill in the Blank" ? null : questionAnswer.SelectedOptionId,
                    FillInTheBlankResponse = questionAnswer.FillInTheBlankResponse,
                    IsCorrect = isCorrect,
                    PointsAwarded = pointsAwarded,
                    SubmittedAt = DateTime.Now
                });
            }

            _context.UserAnswers.AddRange(userAnswers);
            await _context.SaveChangesAsync();

            userQuiz.Score = totalScore;
            _context.UserQuizzes.Update(userQuiz);
            await _context.SaveChangesAsync();

            UserScoreApiController userScoreApiController = new UserScoreApiController(_context);
            await userScoreApiController.SaveUserScore(user.Id, null, quiz.QuizId, totalScore, "Quiz");

            return View("Result", userQuiz);
        }

        [Route("Quiz/Details/{quizId}")]
        public async Task<IActionResult> Details(int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .Include(q => q.Questions)
                .ThenInclude(q => q.FillInTheBlankAnswers)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
            {
                return NotFound("Quiz not found.");
            }

            // ✅ Convert Quiz Model to QuizViewModel
            var quizViewModel = new QuizViewModel
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Duration = quiz.Duration,
                Questions = quiz.Questions.Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    Type = q.Type,
                    Text = q.Text,
                    Points = q.Points,
                    ImageUrl = q.ImageUrl,
                    Options = q.Options.Select(o => new OptionViewModel
                    {
                        OptionId = o.OptionId,
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    }).ToList(),
                    FillInTheBlankAnswers = q.FillInTheBlankAnswers.Select(a => a.Answer).ToList()
                }).ToList()
            };

            return View("~/Views/Quiz/Details.cshtml", quizViewModel);
        }


        // Display quiz result
        public async Task<IActionResult> ReviewQuiz(int userQuizId)
        {
            var userQuiz = await _context.UserQuizzes
                .Include(uq => uq.Quiz)
                .Include(uq => uq.UserAnswers)
                    .ThenInclude(ua => ua.Question)
                    .ThenInclude(q => q.Options) // Load options for MCQs and True/False
                .Include(uq => uq.UserAnswers)
                    .ThenInclude(ua => ua.Question.FillInTheBlankAnswers) // Load correct fill-in-the-blank answers
                .FirstOrDefaultAsync(uq => uq.UserQuizId == userQuizId);

            if (userQuiz == null)
            {
                return NotFound("Quiz result not found.");
            }

            userQuiz.UserAnswers ??= new List<UserAnswer>();

            foreach (var answer in userQuiz.UserAnswers)
            {
                answer.Question ??= new Question();
                answer.Question.Options ??= new List<Option>();
                answer.Question.FillInTheBlankAnswers ??= new List<FillInTheBlankAnswer>();
            }

            return View("Result", userQuiz); // ✅ Ensures `UserQuiz` is passed to `Result.cshtml`
        }

        [Authorize]
        public async Task<IActionResult> QuizResults()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userId = user.Id;

            var userQuizzes = await _context.UserQuizzes
                .Include(q => q.Quiz) // ✅ Load quiz details
                .Include(q => q.UserAnswers)
                    .ThenInclude(a => a.Question) // ✅ Load all Questions
                .Include(q => q.UserAnswers)
                    .ThenInclude(a => a.SelectedOption) // ✅ Load selected options for MCQs and True/False
                .Include(q => q.UserAnswers)
                    .ThenInclude(a => a.Question.FillInTheBlankAnswers) // ✅ Load correct Fill-in-the-Blank answers
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CompletedAt)
                .ToListAsync();

            // ✅ Ensure UserAnswers are properly loaded
            foreach (var quiz in userQuizzes)
            {
                quiz.UserAnswers ??= new List<UserAnswer>();

                foreach (var answer in quiz.UserAnswers)
                {
                    answer.Question ??= new Question();
                    answer.Question.Options ??= new List<Option>();
                    answer.Question.FillInTheBlankAnswers ??= new List<FillInTheBlankAnswer>();

                    // ✅ Handle FIB: If the answer is a Fill-in-the-Blank response, check against all correct answers
                    if (answer.Question.Type == "Fill in the Blank" && !string.IsNullOrWhiteSpace(answer.FillInTheBlankResponse))
                    {
                        var correctAnswers = answer.Question.FillInTheBlankAnswers.Select(f => f.Answer).ToList();
                        answer.IsCorrect = correctAnswers.Any(correct => string.Equals(correct, answer.FillInTheBlankResponse, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            return View(userQuizzes);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.IsActive = isActive;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDueDate(int id, [FromForm] string dueDate)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            if (DateTime.TryParse(dueDate, out var parsedDate))
                quiz.DueDate = parsedDate;

            await _context.SaveChangesAsync();
            return Ok();
        }

            [Authorize]
            [HttpGet]
            public IActionResult KeepAlive()
            {
                return Ok();
            }
       



        [HttpPost]
        public async Task<IActionResult> UpdateAttempts(int id, [FromForm] int attempts)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.NoOfAttempts = Math.Max(1, attempts); // At least 1
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ToggleArchive(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.IsArchived = !quiz.IsArchived;
            await _context.SaveChangesAsync();

            return Ok(new { archived = quiz.IsArchived });
        }



    }
}
