using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AppGameLevelTask
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("AppGameLevel")]
    public int AppGameLevelId { get; set; }
    public virtual AppGameLevel AppGameLevel { get; set; } // Navigation property

    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime SavedDate { get; set; }
    public int RequiredEarningScore { get; set; }
    public string Task { get; set; }
    public virtual ICollection<AppUserTasksStatus> AppUserTasksStatus { get; set; }


}

