using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AppUserTasksStatus
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; } 
    public virtual ApplicationUser ApplicationUser { get; set; } // Navigation property

    [Required]
    [ForeignKey("AppGameLevelTask")]
    public int AppGameLevelTaskId { get; set; }
    public virtual AppGameLevelTask AppGameLevelTask { get; set; } // Navigation property


    public DateTime SavedDate { get; set; }
    public int EarnedScore { get; set; }
    public string TaskStatus { get; set; } //Done,InProcess

}

