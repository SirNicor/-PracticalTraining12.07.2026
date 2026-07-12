namespace DomainCore;

public class LessonGroup
{
    public int LessonID { get; set; }
    public int GroupID { get; set; }
    
    public virtual Lesson Lesson { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
}