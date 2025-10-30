namespace GraceProject.Models
{
    public class StudentSessionModel
    {
        public string CourseID { get; set; } // Course ID
        public string Keyword { get; set; }   // Student ID

        public string DateFilter { get; set; } // "last3months", "monthly", "weekly", "custom"
        public DateTime? StartDate { get; set; } // For custom range
        public DateTime? EndDate { get; set; } // For custom range
    }

}
