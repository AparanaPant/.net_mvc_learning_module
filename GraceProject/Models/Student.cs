using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraceProject.Models;

public class Student: ApplicationUser
{
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public int? GradeId { get; set; }

    [ForeignKey("GradeId")]
    public virtual Grade Grade { get; set; }

}
