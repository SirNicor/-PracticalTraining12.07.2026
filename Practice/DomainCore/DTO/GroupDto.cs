namespace DomainCore.DTO;

public class GroupDto
{
    public int ID { get; set; }
    public string NameGroup { get; set; } = string.Empty;
    public List<string> SubjectsName { get; set; }
}