namespace DomainCore.DTO;

public class TeacherDto
{
    public int ID { get; set; }
    public string FIO { get; set; } = string.Empty;
    public List<string> SubjectsName { get; set; }
}