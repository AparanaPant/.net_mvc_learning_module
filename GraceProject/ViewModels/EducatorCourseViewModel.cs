using GraceProject.Models;

namespace GraceProject.ViewModels
{
    public class EducatorCourseViewModel
    {
        public Course Course { get; set; } // The Course Details
        public List<Session> Sessions { get; set; } // Sessions for this course
        public bool IsAssigned { get; set; } // True if the educator is assigned
    }
}
