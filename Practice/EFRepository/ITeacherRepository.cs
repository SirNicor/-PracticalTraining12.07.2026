using DomainCore.DTO;

namespace EFRepository;

public interface ITeacherRepository
{
    public Task<TeacherDto?> GetTeacherForNameAsync(string name);
    public Task<List<TeacherDto>?> GetAllTeachersAsync();
    public Task<int?> CreateAsync(TeacherDto groupDto);
    public Task<int?> UpdateAsync(TeacherDto groupDto);
    public Task<int?> DeleteAsync(int ID);
}