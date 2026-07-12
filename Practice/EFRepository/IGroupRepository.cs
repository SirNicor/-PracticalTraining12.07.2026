using DomainCore.DTO;

namespace EFRepository;

public interface IGroupRepository
{
    public Task<GroupDto?> GetGroupForNameAsync(string name);
    public Task<List<GroupDto>?> GetAllGroupsAsync();
    public Task<int?> CreateAsync(GroupDto groupDto);
    public Task<int?> UpdateAsync(GroupDto groupDto);
    public Task<int?> DeleteAsync(int ID);
}