using DomainCore;

namespace EFRepository;

public interface ISubjectRepository
{
    public Task<Subject?> GetSubjectsForNameAsync(string name);
    public Task<List<Subject>?> GetAllSubjectsAsync();
    public Task<int?> CreateAsync(Subject subject);
    public Task<int?> UpdateAsync(Subject subject);
    public Task<int?> DeleteAsync(int ID);
}