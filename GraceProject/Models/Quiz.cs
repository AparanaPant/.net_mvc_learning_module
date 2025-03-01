// Models/Quiz.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraceProject.Models
{
    public class Quiz
    {

        [Key] 
        public int QuizId { get; set; }

        [Required]
        public string Title { get; set; }

        public int Duration { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("Course")]
        public string? CourseID { get; set; }

        [ForeignKey("Session")]
        public int? SessionID { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<UserQuiz> UserQuizzes { get; set; }
        public virtual Course? Course { get; set; }
        public virtual Session? Session { get; set; }

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
        [Key]
        public int UserQuizId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Quiz")]
        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int? Score { get; set; }

        public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }


    public class UserAnswer
    {
        [Key] 
        public int UserAnswerId { get; set; }

        public int UserQuizId { get; set; }  // Link to the quiz attempt
        public virtual UserQuiz UserQuiz { get; set; }  // Navigation property

        public int QuestionId { get; set; }  // The question being answered
        public virtual Question Question { get; set; }  // Navigation property

        public int? SelectedOptionId { get; set; }  // MCQ: Chosen option (nullable for non-MCQ)
        public virtual Option SelectedOption { get; set; }  // Navigation property for MCQ

        public string? FillInTheBlankResponse { get; set; }  // Text input (nullable for MCQ)

        public bool IsCorrect { get; set; }  // Was the answer correct?
        public int PointsAwarded { get; set; }  // Points given for this question

        public DateTime SubmittedAt { get; set; } = DateTime.Now;  // Time of submission
    }

}
