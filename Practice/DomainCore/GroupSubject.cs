namespace DomainCore;

public class GroupSubject
{
    public int GroupID { get; set; }
    public int SubjectID { get; set; }
    
    public virtual Group Group { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
}