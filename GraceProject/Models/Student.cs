using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraceProject.Models;

public class Student: ApplicationUser
{
    public ICollection<StudentSession>? StudentSessions { get; set; }
}
