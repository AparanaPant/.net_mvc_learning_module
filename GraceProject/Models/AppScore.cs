using GraceProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AppScore
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    [Required]
    public int InitialScore { get; set; }

}

