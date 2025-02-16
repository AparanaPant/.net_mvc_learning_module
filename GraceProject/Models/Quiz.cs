// Models/Quiz.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }

        [Required]
        public string Title { get; set; }

        public int Duration { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<UserQuiz> UserQuizzes { get; set; }
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }

        [Required]
        public string Text { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public string? ImageUrl { get; set; }

        public virtual ICollection<Option> Options { get; set; }
        public List<FillInTheBlankAnswer> FillInTheBlankAnswers { get; set; }
    }

    public class Option
    {
        public int OptionId { get; set; }

        [Required]
        public string Text { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }

    public class FillInTheBlankAnswer
    {
        public int FillInTheBlankAnswerId { get; set; }
        public string Answer { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }

    public class UserQuiz
    {
        public int UserQuizId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public DateTime CompletedAt { get; set; }

        public int Score { get; set; }
    }
}
