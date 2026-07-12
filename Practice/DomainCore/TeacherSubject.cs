namespace DomainCore;

public class TeacherSubject
{
    public int TeacherID { get; set; }
    public int SubjectID { get; set; }
    
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
}