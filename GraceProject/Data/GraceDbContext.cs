using GraceProject.Data;
using GraceProject.Models;
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

    public GraceDbContext(DbContextOptions<GraceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
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


    }
}
