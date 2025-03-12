using GraceProject.Models;

namespace GraceProject.ViewModels
{
    public class EducatorCourseViewModel
    {
        public Course Course { get; set; } // The Course Details
        public List<Session> Sessions { get; set; } // Sessions for this course
        public bool IsAssigned { get; set; } // True if the educator is assigned
    }

    public class EducatorCourseModel
    {
        public string CourseID { get; set; }  // Course ID
        public string EducatorID { get; set; }  // Educator ID
    }
}
