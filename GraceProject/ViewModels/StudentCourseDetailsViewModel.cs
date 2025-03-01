using GraceProject.Models;
using System.Collections.Generic;

namespace GraceProject.ViewModels
{
    public class StudentCourseDetailsViewModel
    {
        public Course Course { get; set; }

        public List<StudentSessionViewModel> Sessions { get; set; } = new List<StudentSessionViewModel>();
    }
}
