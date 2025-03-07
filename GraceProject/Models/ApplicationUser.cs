using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GraceProject.Models;
public enum Gender
{
    [Display(Name = "Male")]
    Male,

    [Display(Name = "Female")]
    Female,

    [Display(Name = "Prefer not to say")]
    Prefer_Not_To_Say
}

public enum Education
{
    Masters,
    Bachelors,
    PHD
}

public enum University
{
    Auburn,
    AuburnUni,
    AuburnUniversity
}

public class ApplicationUser : IdentityUser
{
    public DateTime? CreatedAt { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(20)")]
    public Gender Gender { get; set; }
    public virtual Address Address { get; set; }

    public int? SchoolID { get; set; }    

    [ForeignKey("SchoolID")]
    public virtual School? School { get; set; }

    public ICollection<UserSchool> UserSchools { get; set; }

}
