namespace GraceProject.ViewModels
{
    public class TeacherGradebookViewModel
    {
        public string CourseTitle { get; set; }
        public string SessionTitle { get; set; }
        public List<StudentQuizResultViewModel> Students { get; set; } = new List<StudentQuizResultViewModel>();
    }

    public class StudentQuizResultViewModel
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public List<EducatorQuizResultViewModel> Quizzes { get; set; } = new List<EducatorQuizResultViewModel>();
        public double TotalScore { get; set; } = 0;
        public double ObtainedScore { get; set; } = 0;
        public double Percentage { get; set; } = 0;
        public string Grade { get; set; } = "N/A";
    }

    public class EducatorQuizResultViewModel
    {
        public string QuizTitle { get; set; }
        public double TotalScore { get; set; } = 0;
        public double ObtainedScore { get; set; } = 0;
    }
}
