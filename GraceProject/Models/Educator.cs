using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public class Educator
{
    [Key]
    public int EducatorID { get; set; }

    public string EducatorUserID { get; set; } = null!;

    public int SchoolID { get; set; }

    public DateTime? JoiningDate { get; set; }

    public virtual ApplicationUser EducatorUser { get; set; } = null!;

    public virtual SchoolInfo SchoolInfo { get; set; } = null!;
}
