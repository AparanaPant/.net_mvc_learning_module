using GraceProject.Models;
using System.Collections.Generic;

namespace GraceProject.ViewModels
{
    public class QuizzesViewModel
    {
        public Course Course { get; set; }

        public DateTime? CurrentTimeCST { get; set; }

        public List<Quiz> DefaultQuizzes { get; set; } = new List<Quiz>();
        public List<Quiz> SessionQuizzes { get; set; } = new List<Quiz>();

    }
}
