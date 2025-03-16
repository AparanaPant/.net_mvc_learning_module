using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraceProject.Models;


public class SlideReadTracking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int SlideId { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public DateTime ReadDateTime { get; set; } = DateTime.UtcNow;

    [Required]
    public int DurationSeconds { get; set; } // Time spent on the slide

    [ForeignKey("SlideId")]
    public virtual Slide Slide { get; set; }
}
