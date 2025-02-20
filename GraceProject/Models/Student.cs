using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public class Student: ApplicationUser
{
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

}
