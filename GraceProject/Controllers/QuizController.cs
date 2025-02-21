// Controllers/QuizController.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraceProject.Models;
using GraceProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GraceProject.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Identity;

namespace GraceProject.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly GraceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(GraceDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List all quizzes
        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return View(quizzes);
        }

        // Create a new quiz
        [HttpGet]
        public IActionResult Create()
        {
            return View(new QuizViewModel { Questions = new List<QuestionViewModel>() });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Educator")]
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
                    Questions = quizViewModel.Questions.Select(q => new Question
                    {
                        Text = q.Text,
                        Type = q.Type,
                        Points = q.Points,
                        ImageUrl = q.Image != null ? UploadImage(q.Image) : q.ImageUrl,
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
                return RedirectToAction(nameof(Index));
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


        private string UploadImage(IFormFile image)
        {
            
            if (image == null || image.Length == 0)
            {
                return null;
            }

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PanelsTemplate", "images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, image.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }
            var imageUrl = $"/PanelsTemplate/images/{image.FileName}";
            return imageUrl;
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
                Questions = quiz.Questions.Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    Type = q.Type,
                    Text = q.Text,
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

        [Authorize(Roles = "Admin,Educator")]
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

        // POST: Quiz/Edit/5
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(QuizViewModel model)
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

                // Update quiz details
                if (!string.IsNullOrEmpty(model.Title))
                {
                    quiz.Title = model.Title;
                }
                if (model.Duration != 0)
                {
                    quiz.Duration = model.Duration;
                }

                // Track question IDs to identify deleted questions
                var updatedQuestionIds = model.Questions.Select(q => q.QuestionId).ToList();
                var existingQuestionIds = quiz.Questions.Select(q => q.QuestionId).ToList();

                // Identify questions to be deleted
                var questionsToDelete = existingQuestionIds.Except(updatedQuestionIds).ToList();

                foreach (var questionId in questionsToDelete)
                {
                    var questionToDelete = quiz.Questions.FirstOrDefault(q => q.QuestionId == questionId);
                    if (questionToDelete != null)
                    {
                        _context.Questions.Remove(questionToDelete);
                    }
                }

                // Update existing questions
                foreach (var questionModel in model.Questions)
                {
                    var existingQuestion = quiz.Questions.FirstOrDefault(q => q.QuestionId == questionModel.QuestionId);
                    if (existingQuestion != null)
                    {
                        // Update only non-null fields
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
                        if (questionModel.Image != null)
                        {
                            // Save the new image and update the ImageUrl
                            var imagePath = Path.Combine("wwwroot/images", questionModel.Image.FileName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await questionModel.Image.CopyToAsync(stream);
                            }
                            existingQuestion.ImageUrl = $"/images/{questionModel.Image.FileName}";
                        }
                        else if (!string.IsNullOrEmpty(questionModel.ImageUrl))
                        {
                            existingQuestion.ImageUrl = questionModel.ImageUrl;
                        }

                        // Update options if they are provided
                        if (questionModel.Options != null)
                        {
                            foreach (var optionModel in questionModel.Options)
                            {
                                var existingOption = existingQuestion.Options.FirstOrDefault(o => o.OptionId == optionModel.OptionId);
                                if (existingOption != null)
                                {
                                    if (!string.IsNullOrEmpty(optionModel.Text))
                                    {
                                        existingOption.Text = optionModel.Text;
                                    }
                                    existingOption.IsCorrect = optionModel.IsCorrect;
                                }
                                else
                                {
                                    // Add new option
                                    existingQuestion.Options.Add(new Option
                                    {
                                        Text = optionModel.Text,
                                        IsCorrect = optionModel.IsCorrect
                                    });
                                }
                            }
                        }

                        // Handle deleted options
                        if (model.Questions.Any(q => q.QuestionId == questionModel.QuestionId && q.OptionsToDelete != null))
                        {
                            foreach (var optionIdToDelete in model.Questions.First(q => q.QuestionId == questionModel.QuestionId).OptionsToDelete)
                            {
                                var optionToDelete = existingQuestion.Options.FirstOrDefault(o => o.OptionId == optionIdToDelete);
                                if (optionToDelete != null)
                                {
                                    existingQuestion.Options.Remove(optionToDelete);
                                }
                            }
                        }

                        // Update fill-in-the-blank answers if they are provided
                        if (questionModel.FillInTheBlankAnswers != null)
                        {
                            // Clear existing answers and add the new ones
                            existingQuestion.FillInTheBlankAnswers.Clear();
                            foreach (var answer in questionModel.FillInTheBlankAnswers)
                            {
                                existingQuestion.FillInTheBlankAnswers.Add(new FillInTheBlankAnswer
                                {
                                    Answer = answer
                                });
                            }
                        }
                    }
                }

                _context.Quizzes.Update(quiz);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Print out model state errors
            foreach (var state in ModelState)
            {
                var key = state.Key;
                var errors = state.Value.Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin,Educator")]
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

        public async Task<IActionResult> Submit(QuizViewModel submitQuizViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

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

            return View("Result", userQuiz);
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

            return View("Result", userQuiz); 
        }

        [Authorize]
        public async Task<IActionResult> QuizResults()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userId = user.Id;

            var userQuizzes = await _context.UserQuizzes
                .Include(q => q.Quiz) 
                .Include(q => q.UserAnswers)
                    .ThenInclude(a => a.Question)
                .Include(q => q.UserAnswers)
                    .ThenInclude(a => a.SelectedOption) 
                .Include(q => q.UserAnswers)
                    .ThenInclude(a => a.Question.FillInTheBlankAnswers) 
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CompletedAt)
                .ToListAsync();

           
            foreach (var quiz in userQuizzes)
            {
                quiz.UserAnswers ??= new List<UserAnswer>();

                foreach (var answer in quiz.UserAnswers)
                {
                    answer.Question ??= new Question();
                    answer.Question.Options ??= new List<Option>();
                    answer.Question.FillInTheBlankAnswers ??= new List<FillInTheBlankAnswer>();

                    
                    if (answer.Question.Type == "Fill in the Blank" && !string.IsNullOrWhiteSpace(answer.FillInTheBlankResponse))
                    {
                        var correctAnswers = answer.Question.FillInTheBlankAnswers.Select(f => f.Answer).ToList();
                        answer.IsCorrect = correctAnswers.Any(correct => string.Equals(correct, answer.FillInTheBlankResponse, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            return View(userQuizzes);
        }



    }
}
