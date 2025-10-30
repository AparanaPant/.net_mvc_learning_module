using GraceProject.Models;
using System.Collections.Generic;

namespace GraceProject.ViewModels
{
    public class QuizAttemptViewModel
    {
        public Quiz Quiz { get; set; }
        public List<Question> Questions { get; set; }
    }
}
