using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GraceProject.Models;

public class Address
{
    [Key]
    public int AddressId { get; set; }

    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; } // Foreign key to AspNetUsers table

    public virtual ApplicationUser ApplicationUser { get; set; } // Navigation property

    [Column(TypeName = "nvarchar(200)")]
    public string StreetAddress { get; set; }


    [Column(TypeName = "nvarchar(200)")]
    public string Country { get; set; } 


    [Column(TypeName = "nvarchar(100)")]
    public string City { get; set; }


    [Column(TypeName = "nvarchar(20)")]
    public string State { get; set; }


    [Column(TypeName = "nvarchar(20)")]
    public string ZIPCode { get; set; }
}
