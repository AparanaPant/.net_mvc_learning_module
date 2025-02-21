// ViewModels/QuizViewModel.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GraceProject.ViewModels
{
    public class QuestionAnswerViewModel
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }  // MCQ Answer
        public string? FillInTheBlankResponse { get; set; }  // Fill-in-the-blank Answer
    }

    public class QuizViewModel
    {
        public int QuizId { get; set; }
        public string Title { get; set; }

        public int Duration { get; set; }

        public List<QuestionAnswerViewModel> QuestionAnswers { get; set; } = new List<QuestionAnswerViewModel>();

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }

    //public class SubmitQuizViewModel
    //{

    //    public int QuizId { get; set; }
    //    public string Title { get; set; }

    //    public int Duration { get; set; }
    //    public List<QuestionAnswerViewModel> QuestionAnswers { get; set; }
    //    public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    //}


    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();
        public List<string> FillInTheBlankAnswers { get; set; } = new List<string>();
        public int Points { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public List<int> OptionsToDelete { get; set; } = new List<int>();

    }

    public class OptionViewModel
    {
        public int OptionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public bool Selected { get; set; }
    }


}
