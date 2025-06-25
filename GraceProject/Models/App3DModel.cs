using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class App3DModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Resource3DModel { get; set; }

    // Foreign key to AppGameLevel
    [ForeignKey("AppGameLevel")]
    public int? AppGameLevelId { get; set; }
    public virtual AppGameLevel AppGameLevel { get; set; }
    [Required]
    public DateTime SavedDate { get; set; }


}

