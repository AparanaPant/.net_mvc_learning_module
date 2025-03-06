namespace GraceProject.Models.ViewModels
{
    public class SessionViewModel
    {
        public int SessionID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string EducatorName { get; set; }
    }
}
