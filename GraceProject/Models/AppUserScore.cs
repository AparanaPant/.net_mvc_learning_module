using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AppUserScore
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; } 
    public virtual ApplicationUser ApplicationUser { get; set; } // Navigation property


    [ForeignKey("AppGameLevelTask")]
    public int? AppGameLevelTaskId { get; set; }
    public virtual AppGameLevelTask AppGameLevelTask { get; set; } // Navigation property

    [ForeignKey("QuizId")]
    public int? QuizId { get; set; }
    public virtual Quiz Quiz { get; set; } // Navigation property


    public DateTime SavedDate { get; set; }
    public int EarnedScore { get; set; }
    public string Description { get; set; }


}

