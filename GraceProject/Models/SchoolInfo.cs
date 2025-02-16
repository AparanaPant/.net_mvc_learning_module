using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public partial class SchoolInfo
{
    [Key]
    public int SchoolID { get; set; }

    public string SchoolName { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual ICollection<Course> Course { get; set; } = new List<Course>();

    public virtual ICollection<Educator> Educator { get; set; } = new List<Educator>();

    public virtual ICollection<Student> Student { get; set; } = new List<Student>();
}