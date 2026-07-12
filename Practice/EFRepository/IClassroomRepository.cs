using DomainCore;

namespace EFRepository;

public interface IClassroomRepository
{
    public Task<Classroom?> GetClassroomForNumberAsync(string number);
    public Task<List<Classroom>?> GetAllClassroomsAsync();
    public Task<int?> CreateAsync(Classroom classroom);
    public Task<int?> UpdateAsync(Classroom classroom);
    public Task<int?> DeleteAsync(int id);
}