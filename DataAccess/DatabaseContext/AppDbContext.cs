using Common.Models;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.DatabaseContext
{
    /// <summary>
    /// Database context
    /// </summary>
    public partial class AppDbContext : DbContext
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<ExamStudent> ExamStudents { get; set; }

        /// <summary>
        /// OnConfiguring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamStudent>()
                .HasOne(es => es.Exam)
                .WithMany(e => e.ExamStudents)
                .HasForeignKey(es => es.ExamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamStudent>()
                .HasOne(es => es.Student)
                .WithMany(s => s.ExamStudents)
                .HasForeignKey(es => es.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Student)
                .WithOne(s => s.Account)
                .HasForeignKey<Student>(s => s.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Teacher)
                .WithOne(t => t.Account)
                .HasForeignKey<Teacher>(t => t.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
            .HasIndex(s => s.Code)
            .IsUnique();

            modelBuilder.Entity<Teacher>()
            .HasIndex(t => t.Code)
            .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
