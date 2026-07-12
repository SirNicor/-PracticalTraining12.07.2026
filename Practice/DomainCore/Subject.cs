namespace DomainCore;

public class Subject
{
    public int ID { get; set; }
    public string NameSubjects { get; set; } = string.Empty;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    public virtual ICollection<GroupSubject> GroupSubjects { get; set; } = new List<GroupSubject>();
}