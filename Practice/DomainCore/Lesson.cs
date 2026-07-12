namespace DomainCore;

public class Lesson
{
    public int ID { get; set; }
    public TimeOnly DateStart { get; set; }
    public int DayOfWeek { get; set; }
    public int NumberOfWeek { get; set; }
    public int TeacherID { get; set; }
    public int ClassroomID { get; set; }
    public int SubjectID { get; set; }
    
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Classroom Classroom { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual ICollection<LessonGroup> LessonGroups { get; set; } = new List<LessonGroup>();
}