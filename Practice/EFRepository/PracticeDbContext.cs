using EFRepository;
using Microsoft.EntityFrameworkCore;
using DomainCore;
using Microsoft.Extensions.Options;

namespace EFRepository;

public class PracticeDbContext : DbContext
{
    public PracticeDbContext(DbContextOptions<PracticeDbContext> options) : base(options)
    {
        
    }

    public PracticeDbContext()
    {
    }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Classroom> Classrooms { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    
    public DbSet<LessonGroup> LessonGroups { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }
    public DbSet<GroupSubject> GroupSubjects { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=practicedb;Username=postgres;Password=admin");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LessonGroup>()
            .HasKey(lg => new { lg.LessonID, lg.GroupID });

        modelBuilder.Entity<TeacherSubject>()
            .HasKey(ts => new { ts.TeacherID, ts.SubjectID });

        modelBuilder.Entity<GroupSubject>()
            .HasKey(gs => new { gs.GroupID, gs.SubjectID });

        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Teacher)
            .WithMany(t => t.Lessons)
            .HasForeignKey(l => l.TeacherID);
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Classroom)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.ClassroomID);
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Subject)
            .WithMany(s => s.Lessons)
            .HasForeignKey(l => l.SubjectID);
        modelBuilder.Entity<TeacherSubject>()
            .HasOne(ts => ts.Teacher)
            .WithMany(t => t.TeacherSubjects)
            .HasForeignKey(ts => ts.TeacherID);
        
        modelBuilder.Entity<TeacherSubject>()
            .HasOne(ts => ts.Subject)
            .WithMany(s => s.TeacherSubjects)
            .HasForeignKey(ts => ts.SubjectID);
        
        modelBuilder.Entity<GroupSubject>()
            .HasOne(gs => gs.Group)
            .WithMany(g => g.GroupSubjects)
            .HasForeignKey(gs => gs.GroupID);
        
        modelBuilder.Entity<GroupSubject>()
            .HasOne(gs => gs.Subject)
            .WithMany(s => s.GroupSubjects)
            .HasForeignKey(gs => gs.SubjectID);

        modelBuilder.Entity<Classroom>()
            .HasIndex(c => c.NumberClassroom)
            .IsUnique();
        modelBuilder.Entity<Subject>()
            .HasIndex(c => c.NameSubjects)
            .IsUnique();
        modelBuilder.Entity<Teacher>()
            .HasIndex(c => c.FIO)
            .IsUnique();
        modelBuilder.Entity<Group>()
            .HasIndex(c => c.NameGroup)
            .IsUnique();
    }
}