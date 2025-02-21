using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraceProject.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        [Required]
        public string GradeName { get; set; } 

        // One-to-Many: A Grade has Many Students
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();

        // Many-to-Many: A Grade has Many Educators
        public virtual ICollection<EducatorGrade> EducatorGrades { get; set; } = new List<EducatorGrade>();
    }

    public class EducatorGrade
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        public string EducatorId { get; set; }

        [Required]
        public int GradeId { get; set; }

        // Navigation Properties
        [ForeignKey("EducatorId")]
        public virtual Educator Educator { get; set; }

        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }
    }
}
