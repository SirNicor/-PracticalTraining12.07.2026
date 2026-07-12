namespace DomainCore;

public class Group
{
    public int ID { get; set; }
    public string NameGroup { get; set; } = string.Empty;
    
    public virtual ICollection<LessonGroup> LessonGroups { get; set; } = new List<LessonGroup>();
    public virtual ICollection<GroupSubject> GroupSubjects { get; set; } = new List<GroupSubject>();
}
