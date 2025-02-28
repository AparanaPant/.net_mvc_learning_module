using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public class Educator: ApplicationUser
{
    public ICollection<EducatorSession>? EducatorSessions { get; set; }
}
