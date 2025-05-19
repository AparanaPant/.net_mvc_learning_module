using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AppQuiz
{
    [Key]
    public int AppQuizId { get; set; }

    public int OriginalQuizId { get; set; }  // Reference to the original Quiz
    [ForeignKey("OriginalQuizId")]
    public virtual Quiz OriginalQuiz { get; set; }

    [Required]
    public int AppModuleId { get; set; }  // Placeholder for future AppModule table

    [Required]
    public string Title { get; set; }

    public int Duration { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = false;

    public bool IsArchived { get; set; } = false;

    public virtual ICollection<AppQuestion> Questions { get; set; } = new List<AppQuestion>();
}

public class AppQuestion
{
    [Key]
    public int AppQuestionId { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public int Points { get; set; }

    public string? ImageUrl { get; set; }

    [ForeignKey("AppQuiz")]
    public int AppQuizId { get; set; }

    public virtual AppQuiz AppQuiz { get; set; }

    public virtual ICollection<AppOption> Options { get; set; } = new List<AppOption>();
}

public class AppOption
{
    [Key]
    public int AppOptionId { get; set; }

    [Required]
    public string Text { get; set; }

    public bool IsCorrect { get; set; }

    [ForeignKey("AppQuestion")]
    public int AppQuestionId { get; set; }

    public virtual AppQuestion AppQuestion { get; set; }
}
