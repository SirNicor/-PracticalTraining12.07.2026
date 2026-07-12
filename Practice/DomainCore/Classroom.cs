namespace DomainCore;

public class Classroom
{
    public int ID { get; set; }
    public string NumberClassroom { get; set; } = string.Empty;
    
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}