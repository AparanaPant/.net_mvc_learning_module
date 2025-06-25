using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AppLearningSlide
{
    [Key]
    public int Id { get; set; }

    // Reference to App3DModel
    [ForeignKey("App3DModel")]
    public int App3DModelId { get; set; }
    public App3DModel App3DModel { get; set; }

    // Reference to GameLevel
    [ForeignKey("AppGameLevel")]
    public int AppGameLevelId { get; set; }
    public AppGameLevel AppGameLevel { get; set; }  // Navigation property

    public string Title { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }


}

