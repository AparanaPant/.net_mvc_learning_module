using GraceProject.Models;

namespace GraceProject.ViewModels
{
    public class StudentCourseViewModel
    {
        public Course Course { get; set; }
    }

    public class SchoolCourseRequestModel
    {
        public int Id { get; set; } // School ID
        public string? CourseID { get; set; } // Course ID (nullable)
        public string DateFilter { get; set; } // Date filter type (e.g., "monthly", "weekly")
        public DateTime? StartDate { get; set; } // Custom Start Date (if selected)
        public DateTime? EndDate { get; set; } // Custom End Date (if selected)
    }

}
