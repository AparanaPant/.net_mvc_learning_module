using GraceProject.Data;
using GraceProject.Models;
using Microsoft.EntityFrameworkCore;

public class EducatorService
{
    private readonly GraceDbContext _context;

    public EducatorService(GraceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterEducatorToCourse(string educatorId, string courseId)
    {
        if (string.IsNullOrEmpty(educatorId) || string.IsNullOrEmpty(courseId))
        {
            return false; // Invalid input
        }

        var existingSession = await _context.EducatorSession
            .Include(es => es.Session)
            .Where(es => es.EducatorID == educatorId && es.Session.CourseID == courseId)
            .FirstOrDefaultAsync();

        if (existingSession != null)
        {
            return false; // Already assigned to this course
        }

       
        var newSession = new Session
        {
            CourseID = courseId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(6),
        };

        _context.Session.Add(newSession);
        await _context.SaveChangesAsync(); // Save new session

      
        var educatorSession = new EducatorSession
        {
            EducatorID = educatorId,
            SessionID = newSession.SessionID
        };

        _context.EducatorSession.Add(educatorSession);
        await _context.SaveChangesAsync(); // Save educator assignment

        return true; // Success
    }
}
