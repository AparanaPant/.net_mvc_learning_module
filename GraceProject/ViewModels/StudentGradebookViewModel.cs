public class GradebookViewModel
{
    public string CourseTitle { get; set; }
    public List<QuizResultViewModel> Quizzes { get; set; } = new List<QuizResultViewModel>();
    public int TotalScore { get; set; }
    public int ObtainedScore { get; set; }
    public double Percentage => TotalScore > 0 ? Math.Round((double)ObtainedScore / TotalScore * 100, 2) : 0;
}


public class QuizResultViewModel
{
    public string QuizTitle { get; set; }
    public int TotalScore { get; set; }
    public int ObtainedScore { get; set; }
    public double Percentage => TotalScore > 0 ? Math.Round((double)ObtainedScore / TotalScore * 100, 2) : 0;
}
