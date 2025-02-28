using GraceProject.Data;
using GraceProject.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GraceProject.Data;

public class GraceDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Address> Address { get; set; }
    public DbSet<Module> Module { get; set; }
    public DbSet<Slide> Slide { get; set; }
    public DbSet<SlideSection> SlideSection { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<SchoolAddress> SchoolAddresses { get; set; }
    public DbSet<UserSchool> UserSchools { get; set; }
    public DbSet<Quiz> Quizzes { get; set; } 
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; } 
    public DbSet<UserQuiz> UserQuizzes { get; set; }

    public DbSet<Course> Course { get; set; }

    public DbSet<Educator> Educator { get; set; }

    public DbSet<Enrollment> Enrollment { get; set; }

    public DbSet<Student> Student { get; set; }

    public DbSet<UserAnswer> UserAnswers { get; set; }

    public DbSet<Session> Session { get; set; }

    public DbSet<EducatorSession> EducatorSession { get; set; }
    public DbSet<StudentSession> StudentSessions { get; set; }


    public GraceDbContext(DbContextOptions<GraceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserQuiz>()
            .HasMany(uq => uq.UserAnswers)
            .WithOne(ua => ua.UserQuiz)
            .HasForeignKey(ua => ua.UserQuizId)
            .OnDelete(DeleteBehavior.Restrict); 

       
        builder.Entity<UserQuiz>()
            .HasOne(uq => uq.Quiz)
            .WithMany(q => q.UserQuizzes)
            .HasForeignKey(uq => uq.QuizId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Entity<UserAnswer>()
            .HasOne(ua => ua.Question)
            .WithMany()
            .HasForeignKey(ua => ua.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        
        builder.Entity<UserAnswer>()
            .HasOne(ua => ua.SelectedOption)
            .WithMany()
            .HasForeignKey(ua => ua.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<ApplicationUser>()
            .HasDiscriminator<string>("UserType")
            .HasValue<ApplicationUser>("ApplicationUser")
            .HasValue<Student>("Student")
            .HasValue<Educator>("Educator");

        builder.Entity<Address>().ToTable("Address");
        builder.Entity<Module>().ToTable("Module");
        builder.Entity<Slide>().ToTable("Slide");
        builder.Entity<SlideSection>().ToTable("SlideSection");
        builder.Entity<School>().ToTable("Schools");
        builder.Entity<SchoolAddress>().ToTable("SchoolAddresses");
        builder.Entity<School>()
            .HasMany(s => s.SchoolAddresses)
            .WithOne(sa => sa.School)
            .HasForeignKey(sa => sa.SchoolID);

        builder.Entity<UserSchool>()
           .HasOne(us => us.School)
           .WithMany(s => s.UserSchools)
           .HasForeignKey(us => us.SchoolID);

        builder.Entity<Quiz>().ToTable("Quizzes");
        builder.Entity<Question>().ToTable("Questions");
        builder.Entity<Option>().ToTable("Options");
        builder.Entity<UserQuiz>().ToTable("UserQuizzes");

        builder.Entity<Quiz>()
            .HasMany(q => q.Questions)
            .WithOne(q => q.Quiz)
            .HasForeignKey(q => q.QuizId);

        builder.Entity<Question>()
            .HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId);

        builder.Entity<UserQuiz>()
            .HasOne(uq => uq.Quiz)
            .WithMany(q => q.UserQuizzes)
            .HasForeignKey(uq => uq.QuizId);

        builder.Entity<UserQuiz>()
            .HasOne(uq => uq.User)
            .WithMany()
            .HasForeignKey(uq => uq.UserId);
        
        //builder.Entity<SchoolInfo>().ToTable("SchoolInfo");
        //builder.Entity<SchoolInfo>(entity =>
        //{
            // Define primary key
            //entity.HasKey(e => e.SchoolID).HasName("PK_SchoolInfo");

            //// Map to table name
            //entity.ToTable("SchoolInfo");

            //// Define properties
            //entity.Property(e => e.SchoolID)
            //    .HasColumnName("SchoolID")
            //    .IsRequired(); // Primary key, required

            //entity.Property(e => e.SchoolName)
            //    .HasColumnName("SchoolName")
            //    .IsRequired()
            //    .HasMaxLength(255); // You can adjust the max length as per your requirements

            //entity.Property(e => e.Country)
            //    .HasColumnName("Country")
            //    .IsRequired()
            //    .HasMaxLength(100); // You can adjust the max length as per your requirements

            // Define relationships
            //entity.HasMany(e => e.Course)
            //    .WithOne(c => c.SchoolInfo)
            //    .HasForeignKey(c => c.SchoolID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //entity.HasMany(e => e.Educator)
            //    .WithOne(ed => ed.SchoolInfo)
            //    .HasForeignKey(ed => ed.SchoolID)
            //    .OnDelete(DeleteBehavior.Restrict);

            //entity.HasMany(e => e.Student)
            //    .WithOne(s => s.SchoolInfo)
            //    .HasForeignKey(s => s.SchoolID)
        //    //    .OnDelete(DeleteBehavior.Restrict);
        //});
    }
}
