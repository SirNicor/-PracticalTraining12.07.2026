using DomainCore;
using DomainCore.DTO;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class LessonRepository : ILessonRepository
{
    private readonly PracticeDbContext _context;

    public LessonRepository(PracticeDbContext context)
    {
        _context = context;
    }
    
    public async Task<LessonDTOInfo?> GetLessonInfoForIDAsync(int ID)
    {
        return await _context.Lessons.Where(l => l.ID == ID).Select(l => new LessonDTOInfo()
        {
            DateStart = l.DateStart,
            DayOfWeek = l.DayOfWeek,
            NumberOfWeek = l.NumberOfWeek,
        }).FirstOrDefaultAsync();
    }

    public async Task<List<LessonDTOInfo>?> GetLessonInfoForNameGroupAsync(string name)
    {
        return await _context.Lessons.Where(l => l.LessonGroups.Any(lg => lg.Group.NameGroup == name)).Select(l => new LessonDTOInfo()
        {
            DateStart = l.DateStart,
            DayOfWeek = l.DayOfWeek,
            NumberOfWeek = l.NumberOfWeek,
        }).ToListAsync();
    }

    public async Task<List<LessonDTOInfo>?> GetLessonInfoForNameTeacherAsync(string name)
    {
        return await _context.Lessons.Where(l => l.Teacher.FIO == name).Select(l => new LessonDTOInfo()
        {
            DateStart = l.DateStart,
            DayOfWeek = l.DayOfWeek,
            NumberOfWeek = l.NumberOfWeek,
        }).ToListAsync();
    }

    public async Task<List<LessonDTOInfo>> GetLessonInfoForNumberClassroomAsync(string name)
    {
        return await _context.Lessons.Where(l => l.Classroom.NumberClassroom == name).Select(l => new LessonDTOInfo()
        {
            DateStart = l.DateStart,
            DayOfWeek = l.DayOfWeek,
            NumberOfWeek = l.NumberOfWeek,
        }).ToListAsync();
    }

        public async Task<int?> CreateLessonAsync(LessonDTOCreate dto) 
        {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await ConflictSearch(dto))
            {
                await transaction.RollbackAsync();
                return null;
            }
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.FIO == dto.TeacherName);
            var classroom = await _context.Classrooms.FirstOrDefaultAsync(c => c.NumberClassroom == dto.ClassroomNumber);
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.NameSubjects == dto.SubjectName);

            if (teacher == null || classroom == null || subject == null)
            {
                await transaction.RollbackAsync();
                return null;
            }
            var lesson = new Lesson
            {
                DateStart = dto.DateStart,
                DayOfWeek = dto.DayOfWeek,
                NumberOfWeek = dto.NumberOfWeek,
                TeacherID = teacher.ID,
                ClassroomID = classroom.ID,
                SubjectID = subject.ID
            };
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            var groups = await _context.Groups
                .Where(g => dto.GroupNames.Contains(g.NameGroup))
                .ToListAsync();
            if (dto.GroupNames.Count != groups.Count)
            {
                return null;
            }
            foreach (var group in groups)
            {
                _context.LessonGroups.Add(new LessonGroup
                {
                    LessonID = lesson.ID,
                    GroupID = group.ID
                });
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return lesson.ID;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<int?> UpdateLessonAsync(LessonDTOCreate dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await ConflictSearch(dto))
            {
                await transaction.RollbackAsync();
                return null;
            }
            var lesson = await _context.Lessons
                .Include(l => l.LessonGroups)
                .FirstOrDefaultAsync(l => l.ID == dto.ID);
            if (lesson == null)
            {
                await transaction.RollbackAsync();
                return null;
            }
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.FIO == dto.TeacherName);
            var classroom = await _context.Classrooms.FirstOrDefaultAsync(c => c.NumberClassroom == dto.ClassroomNumber);
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.NameSubjects == dto.SubjectName);
            if (teacher == null || classroom == null || subject == null)
            {
                await transaction.RollbackAsync();
                return null;
            }
            lesson.DateStart = dto.DateStart;
            lesson.DayOfWeek = dto.DayOfWeek;
            lesson.NumberOfWeek = dto.NumberOfWeek;
            lesson.TeacherID = teacher.ID;
            lesson.ClassroomID = classroom.ID;
            lesson.SubjectID = subject.ID;
            _context.LessonGroups.RemoveRange(lesson.LessonGroups);
            await _context.SaveChangesAsync();

            var groups = await _context.Groups
                .Where(g => dto.GroupNames.Contains(g.NameGroup))
                .ToListAsync();
            if (dto.GroupNames.Count != groups.Count)
            {
                return null;
            }
            foreach (var group in groups)
            {
                _context.LessonGroups.Add(new LessonGroup
                {
                    LessonID = lesson.ID,
                    GroupID = group.ID
                });
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return lesson.ID;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<int?> DeleteLessonAsync(int ID)
    {
        var lesson = await _context.Lessons
            .Include(l => l.LessonGroups)
            .FirstOrDefaultAsync(l => l.ID == ID);
        if (lesson == null)
        {
            return null;
        }
        _context.LessonGroups.RemoveRange(lesson.LessonGroups);
        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return ID;
    }

    public async Task<bool> ConflictSearch(LessonDTOCreate dto)
    {
        var groupsHaveSubject = await _context.Groups
            .Where(g => dto.GroupNames.Contains(g.NameGroup))
            .AllAsync(g => g.GroupSubjects.Any(gs => gs.Subject.NameSubjects == dto.SubjectName));
        if (!groupsHaveSubject)
        {
            return false;  
        }
        var teachersHaveSubject = await _context.TeacherSubjects.AnyAsync(ts => ts.Subject.NameSubjects == dto.SubjectName && ts.Teacher.FIO == dto.TeacherName);
        if (!teachersHaveSubject)
        {
            return false;  
        }
        var dateEndDto = dto.DateStart.AddMinutes(90);
        var dateStartMinus90 = dto.DateStart.AddMinutes(-90);
        var result = await _context.Lessons.AnyAsync(l =>
            l.ID != dto.ID &&  
            l.DateStart < dateEndDto &&
            l.DateStart > dateStartMinus90 &&
            l.NumberOfWeek == dto.NumberOfWeek &&
            l.DayOfWeek == dto.DayOfWeek &&
            (l.Classroom.NumberClassroom == dto.ClassroomNumber ||
             l.Teacher.FIO == dto.TeacherName ||
             l.LessonGroups.Any(lg => dto.GroupNames.Contains(lg.Group.NameGroup))));
        Console.WriteLine(result);
        return !result;
    }
}