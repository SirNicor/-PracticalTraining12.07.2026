namespace DomainCore;

public class Teacher
{
    public int ID { get; set; }
    public string FIO { get; set; } = string.Empty;
    
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}