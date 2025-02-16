using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public class Enrollment
{
    [Key]
    public int EnrollmentID { get; set; }
    public string CourseID { get; set; } = null!;

    public string StudentUserID { get; set; } = null!;

    public DateTime? JoiningDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ApplicationUser StudentUser { get; set; } = null!;
}
