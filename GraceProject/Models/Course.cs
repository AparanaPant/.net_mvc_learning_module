using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public class Course
{
    [Key]
    public string CourseID { get; set; } = null!;

    public string? Title { get; set; }

    public int? Credits { get; set; }

    public string? EducatorUserID { get; set; }

    public int? SchoolID { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ApplicationUser? EducatorUser { get; set; }

    public virtual SchoolInfo? SchoolInfo { get; set; }
}
