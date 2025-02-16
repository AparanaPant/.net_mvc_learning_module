using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models
{
    public class SlideSection
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Slide")]
        public int SlideId { get; set; }


        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; } // Foreign key to AspNetUsers table

        public virtual ApplicationUser ApplicationUser { get; set; } // Navigation property

        [Column(TypeName = "nvarchar(1000)")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime SavedDateTime { get; set; }

        // We can add some controllers such as clickable image, chart, table, flying over items, drag and drop items, image, video
        //each controller might have some parameters and we can store them as JSON string in the field of ControllerParameters
        public string ControllerName { get; set; }
        public string ControllerParameters { get; set; }

        public bool TextToVoice { get; set; }

        public int SlideSectionOrder { get; set; }


    }
}
