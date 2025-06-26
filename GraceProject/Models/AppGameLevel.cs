using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace GraceProject.Models;
public class AppGameLevel
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime SavedDate { get; set; }
    public int RequiredScore { get; set; }

    public virtual ICollection<App3DModel> App3DModels { get; set; }
    public virtual ICollection<AppEquipment> AppEquipments { get; set; }
    public virtual ICollection<AppGameLevelTask> AppGameLevelTasks { get; set; }
    public virtual ICollection<AppLearningSlide> AppLearningSlides { get; set; }


}

