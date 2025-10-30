using GraceProject.Data;
using GraceProject.Models;
using Microsoft.EntityFrameworkCore;

public class StudentService
{
    private readonly GraceDbContext _context;

    public StudentService(GraceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterStudentToSession(string studentId, int sessionId)
    {
        // Check if student is already registered
        var existingEnrollment = await _context.StudentSessions
            .FirstOrDefaultAsync(ss => ss.StudentID == studentId && ss.SessionID == sessionId);

        if (existingEnrollment == null)
        {
            var studentSession = new StudentSession
            {
                StudentID = studentId,
                SessionID = sessionId,
                RegistrationDate = DateTime.UtcNow
            };

            _context.StudentSessions.Add(studentSession);
            await _context.SaveChangesAsync();
            return true; // Successfully registered
        }

        return false; // Already registered
    }
}
