using DomainCore.DTO;

namespace EFRepository;

public interface ILessonRepository
{
    public Task<LessonDTOInfo?> GetLessonInfoForIDAsync(int ID);
    public Task<List<LessonDTOInfo>?> GetLessonInfoForNameGroupAsync(string name);
    public Task<List<LessonDTOInfo>?> GetLessonInfoForNameTeacherAsync(string name);
    public Task<List<LessonDTOInfo>?> GetLessonInfoForNumberClassroomAsync(string name);
    public Task<int?> CreateLessonAsync(LessonDTOCreate dto);
    public Task<int?> UpdateLessonAsync(LessonDTOCreate dto);
    public Task<int?> DeleteLessonAsync(int ID);
    public Task<bool> ConflictSearch(LessonDTOCreate dto);
}