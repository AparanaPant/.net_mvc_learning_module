using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraceProject.Models;

public class Course
{
    [Key]
    public string CourseID { get; set; } = null!;

    public string? Title { get; set; }

    public int? Credits { get; set; }

    public virtual ICollection<CourseEducator> CourseEducators { get; set; } = new List<CourseEducator>();


    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

}

public class CourseEducator
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }  // Primary key

    public DateTime JoiningDate { get; set; } = DateTime.Now;

    [Required]
    public string CourseID { get; set; } = null!;

    [Required]
    public string EducatorUserID { get; set; } = null!;

    // Navigation properties
    [ForeignKey("CourseID")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("EducatorUserID")]
    public virtual Educator Educator { get; set; } = null!;
}
