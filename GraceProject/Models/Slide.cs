using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models
{
    public class Slide
    {
        [Key]
        public int Id { get; set; }

        public ICollection<SlideSection> SlideSections { get; set; }

        [Required]
        public string SlideSectionsType { get; set; }

        //public string SlideType { get; set; }

        

        [Required]
        public int NumSlideSections { get; set; }

        [ForeignKey("Module")]
        public int ModuleId { get; set; }


        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; } // Foreign key to AspNetUsers table

        public virtual ApplicationUser ApplicationUser { get; set; } // Navigation property

        [Required]
        [Column(TypeName = "nvarchar(1000)")]
        public string Title { get; set; }

        public bool DisplayTitle { get; set; }

        [Required]
        public DateTime SavedDateTime { get; set; }

        public string ShortTitle { get; set; }

        public string ThumbIcon { get; set; }
        public bool ForwardButton { get; set; }
        public bool BackwardButton { get; set; }

        public int SlideOrder { get; set; }

        public bool DisplayThumbnailSlide { get; set; }


    }
}
