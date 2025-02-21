using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public class Educator: ApplicationUser
{
    public virtual ICollection<CourseEducator> CourseEducators { get; set; } = new List<CourseEducator>();
    public virtual ICollection<EducatorGrade> EducatorGrades { get; set; } = new List<EducatorGrade>();
}
