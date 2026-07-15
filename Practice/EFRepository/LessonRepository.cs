using DomainCore;
using DomainCore.DTO;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class LessonRepository : ILessonRepository
{
    private readonly IDbContextFactory<PracticeDbContext> _dbFactory;

    public LessonRepository(IDbContextFactory<PracticeDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }
    
    public async Task<LessonDTOInfo?> GetLessonInfoForIDAsync(int ID)
    {
        await using var context = _dbFactory.CreateDbContext();
        return await context.Lessons.Where(l => l.ID == ID).Select(l => new LessonDTOInfo()
        {
            DateStart = l.DateStart,
            DayOfWeek = l.DayOfWeek,
            NumberOfWeek = l.NumberOfWeek,
        }).FirstOrDefaultAsync();
    }

    public async Task<List<LessonDTOInfo>?> GetLessonInfoForNameGroupAsync(string name)
    {
        await using var context = _dbFactory.CreateDbContext();
        return await context.Lessons.Where(l => l.LessonGroups.Any(lg => lg.Group.NameGroup == name)).Select(l => new LessonDTOInfo()
        {
            DateStart = l.DateStart,
            DayOfWeek = l.DayOfWeek,
            NumberOfWeek = l.NumberOfWeek,
        }).ToListAsync();
    }

    public async Task<List<LessonDTOInfo>?> GetLessonInfoForNameTeacherAsync(string name)
    {
        await using var context = _dbFactory.CreateDbContext();
        return await context.Lessons.Where(l => l.Teacher.FIO == name).Select(l => new LessonDTOInfo()
        {
            DateStart = l.DateStart,
            DayOfWeek = l.DayOfWeek,
            NumberOfWeek = l.NumberOfWeek,
        }).ToListAsync();
    }

    public async Task<List<LessonDTOInfo>> GetLessonInfoForNumberClassroomAsync(string name)
    {
        await using var context = _dbFactory.CreateDbContext();
        return await context.Lessons.Where(l => l.Classroom.NumberClassroom == name).Select(l => new LessonDTOInfo()
        {
            DateStart = l.DateStart,
            DayOfWeek = l.DayOfWeek,
            NumberOfWeek = l.NumberOfWeek,
        }).ToListAsync();
    }

        public async Task<int?> CreateLessonAsync(LessonDTOCreate dto) 
        {
            await using var context = _dbFactory.CreateDbContext();
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (!await ConflictSearch(dto))
            {
                await transaction.RollbackAsync();
                return null;
            }
            var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.FIO == dto.TeacherName);
            var classroom = await context.Classrooms.FirstOrDefaultAsync(c => c.NumberClassroom == dto.ClassroomNumber);
            var subject = await context.Subjects.FirstOrDefaultAsync(s => s.NameSubjects == dto.SubjectName);

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
            context.Lessons.Add(lesson);
            await context.SaveChangesAsync();
            var groups = await context.Groups
                .Where(g => dto.GroupNames.Contains(g.NameGroup))
                .ToListAsync();
            if (dto.GroupNames.Count != groups.Count)
            {
                return null;
            }
            foreach (var group in groups)
            {
                context.LessonGroups.Add(new LessonGroup
                {
                    LessonID = lesson.ID,
                    GroupID = group.ID
                });
            }
            await context.SaveChangesAsync();
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
        await using var context = _dbFactory.CreateDbContext();
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (!await ConflictSearch(dto))
            {
                await transaction.RollbackAsync();
                return null;
            }
            var lesson = await context.Lessons
                .Include(l => l.LessonGroups)
                .FirstOrDefaultAsync(l => l.ID == dto.ID);
            if (lesson == null)
            {
                await transaction.RollbackAsync();
                return null;
            }
            var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.FIO == dto.TeacherName);
            var classroom = await context.Classrooms.FirstOrDefaultAsync(c => c.NumberClassroom == dto.ClassroomNumber);
            var subject = await context.Subjects.FirstOrDefaultAsync(s => s.NameSubjects == dto.SubjectName);
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
            context.LessonGroups.RemoveRange(lesson.LessonGroups);
            await context.SaveChangesAsync();

            var groups = await context.Groups
                .Where(g => dto.GroupNames.Contains(g.NameGroup))
                .ToListAsync();
            if (dto.GroupNames.Count != groups.Count)
            {
                return null;
            }
            foreach (var group in groups)
            {
                context.LessonGroups.Add(new LessonGroup
                {
                    LessonID = lesson.ID,
                    GroupID = group.ID
                });
            }
            await context.SaveChangesAsync();
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
        await using var context = _dbFactory.CreateDbContext();
        var lesson = await context.Lessons
            .Include(l => l.LessonGroups)
            .FirstOrDefaultAsync(l => l.ID == ID);
        if (lesson == null)
        {
            return null;
        }
        context.LessonGroups.RemoveRange(lesson.LessonGroups);
        context.Lessons.Remove(lesson);
        await context.SaveChangesAsync();
        return ID;
    }

    public async Task<bool> ConflictSearch(LessonDTOCreate dto)
    {
        await using var context = _dbFactory.CreateDbContext();
        var groupsHaveSubject = await context.Groups
            .Where(g => dto.GroupNames.Contains(g.NameGroup))
            .AllAsync(g => g.GroupSubjects.Any(gs => gs.Subject.NameSubjects == dto.SubjectName));
        if (!groupsHaveSubject)
        {
            return false;  
        }
        var teachersHaveSubject = await context.TeacherSubjects.AnyAsync(ts => ts.Subject.NameSubjects == dto.SubjectName && ts.Teacher.FIO == dto.TeacherName);
        if (!teachersHaveSubject)
        {
            return false;  
        }
        var dateEndDto = dto.DateStart.AddMinutes(90);
        var dateStartMinus90 = dto.DateStart.AddMinutes(-90);
        var result = await context.Lessons.AnyAsync(l =>
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