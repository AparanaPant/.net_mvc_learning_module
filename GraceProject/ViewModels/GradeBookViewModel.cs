namespace GraceProject.Models.ViewModels
{
    public class GradeBookViewModel
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public List<QuizScoreViewModel> QuizScores { get; set; } = new List<QuizScoreViewModel>();
    }

    public class QuizScoreViewModel
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int? Score { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
