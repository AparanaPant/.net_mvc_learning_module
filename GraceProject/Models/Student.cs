using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public class Student
{
    [Key]
    public int StudentID { get; set; }
    public string StudentUserID { get; set; } = null!;

    public int SchoolID { get; set; }

    public DateTime? JoiningDate { get; set; }

    public virtual SchoolInfo SchoolInfo { get; set; } = null!;

    public virtual ApplicationUser StudentUser { get; set; } = null!;
}
