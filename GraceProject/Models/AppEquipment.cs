using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AppEquipment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Resource3DModel { get; set; }

    public string Description { get; set; }

    //The equipment should be enabled at what Gamelevel
    [ForeignKey("AppGameLevel")]
    public int AppGameLevelId { get; set; }
    public AppGameLevel AppGameLevel { get; set; }  // Navigation property

    //related 3DModel
    [ForeignKey("App3DModel")]
    public int App3DModelId { get; set; }
    public App3DModel App3DModel { get; set; }  


    public DateTime SavedDate { get; set; }
    public int RequiredScore { get; set; }

}

