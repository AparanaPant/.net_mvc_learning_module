using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.DependencyResolver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraceProject.Models;

public class Course
{
    [Key]
    public string CourseID { get; set; } = null!;

    public string? Title { get; set; }

    public int? Credits { get; set; }
    public ICollection<Module>? Modules { get; set; }
    public ICollection<Quiz>? Quizzes { get; set; }
    public ICollection<Session>? Sessions { get; set; }


}

public class Session
{
    [Key]
    public int SessionID { get; set; }

    [ForeignKey("Course")]
    public string CourseID { get; set; } = null!;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation properties
    public Course? Course { get; set; }
    public ICollection<StudentSession>? StudentSessions { get; set; }
    public ICollection<EducatorSession>? EducatorSessions { get; set; }
}

public class StudentSession
{
    [Key]
    public int StudentSessionID { get; set; }

    [ForeignKey("Student")]
    public string StudentID { get; set; }

    [ForeignKey("Session")]
    public int SessionID { get; set; }

    // Navigation properties
    public Student? Student { get; set; }
    public Session? Session { get; set; }
    public DateTime RegistrationDate { get; internal set; }
}

public class EducatorSession
{
    [Key]
    public int EducatorSessionID { get; set; }

    [ForeignKey("Educator")]
    public string EducatorID { get; set; } = null!;  

    [ForeignKey("Session")]
    public int SessionID { get; set; }

    public Educator? Educator { get; set; }
    public Session? Session { get; set; }
}

