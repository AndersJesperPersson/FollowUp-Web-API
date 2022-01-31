namespace SurveyAPI
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using SurveyAPI.Models;

    public class ApplicationDbContext : IdentityDbContext

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SurveysAnswers>()
                .HasKey(x => new { x.SurveyId, x.AnswerId });

            modelBuilder.Entity<SurveysQuestions>()
    .HasKey(x => new { x.SurveyId, x.QuestionId });

            modelBuilder.Entity<MissionSurveys>()
.HasKey(x => new { x.MissionId, x.SurveyId });

            modelBuilder.Entity<MissionEmployees>()
.HasKey(x => new { x.MissionId, x.EmployeeId });


            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<SurveysQuestions> SurveysQuestions { get; set; }
        public DbSet<SurveysAnswers> SurveysAnswers { get; set; }

        public DbSet<MissionSurveys> MissionSurveys { get; set; }
        public DbSet<MissionEmployees> MissionEmployees { get; set; }

    }
}
