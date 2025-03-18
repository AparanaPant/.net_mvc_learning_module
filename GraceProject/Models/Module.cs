using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models
{
    public class Module
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; } // Foreign key to AspNetUsers table

        [ForeignKey("Course")]
        public string CourseId { get; set; } 

        public virtual ApplicationUser ApplicationUser { get; set; } // Navigation property

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        [Required]
        [Display(Name = "Saved Date and Time")]
        public DateTime SavedDateTime { get; set; }


        // Self-referencing property
        [ForeignKey("ParentModule")]
        public int? ParentModuleId { get; set; }
        public virtual Module ParentModule { get; set; }
        public virtual ICollection<Module> ChildModules { get; set; }

    }
}
