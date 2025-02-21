using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraceProject.Models;

public class Enrollment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; }
    public string CourseID { get; set; } = null!;

    public string StudentUserID { get; set; } = null!;

    public DateTime JoiningDate { get; set; } = DateTime.Now;

    [ForeignKey("CourseID")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("StudentUserID")]
    public virtual Student StudentUser { get; set; } = null!;
}
