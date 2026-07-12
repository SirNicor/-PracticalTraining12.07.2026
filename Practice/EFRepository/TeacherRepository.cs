using DomainCore;
using DomainCore.DTO;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class TeacherRepository : ITeacherRepository
{
    private readonly PracticeDbContext _context;

    public TeacherRepository(PracticeDbContext context)
    {
        _context = context;
    }

    public async Task<TeacherDto?> GetTeacherForNameAsync(string name)
    {
        return await _context.Teachers
            .Where(t => t.FIO == name)
            .Select(t => new TeacherDto
            {
                ID = t.ID,
                FIO = t.FIO,
                SubjectsName = t.TeacherSubjects.Select(ts => ts.Subject.NameSubjects).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task<List<TeacherDto>?> GetAllTeachersAsync()
    {
        return await _context.Teachers.Select(t => new TeacherDto
        {
            ID = t.ID,
            FIO = t.FIO,
            SubjectsName = t.TeacherSubjects.Select(ts => ts.Subject.NameSubjects).ToList()
        }).ToListAsync();
    }

    public async Task<int?> CreateAsync(TeacherDto teacherDto)
    {
        try
        {
            var teacher = new Teacher
            {
                FIO = teacherDto.FIO
            };

            var subjects = await _context.Subjects
                .Where(s => teacherDto.SubjectsName.Contains(s.NameSubjects))
                .ToListAsync();
            
            if (subjects.Count != teacherDto.SubjectsName.Count)
            {
                return null;
            }
            foreach (var subject in subjects)
            {
                teacher.TeacherSubjects.Add(new TeacherSubject
                {
                    SubjectID = subject.ID
                });
            }
            
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return teacher.ID;
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public async Task<int?> UpdateAsync(TeacherDto teacherDto)
    {
        try
        {
            var teacher = await _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.ID == teacherDto.ID);

            if (teacher == null)
            {
                return null;
            }

            teacher.FIO = teacherDto.FIO;

            teacher.TeacherSubjects.Clear();

            var subjects = await _context.Subjects
                .Where(s => teacherDto.SubjectsName.Contains(s.NameSubjects))
                .ToListAsync();
            if (subjects.Count != teacherDto.SubjectsName.Count)
            {
                return null;
            }
            foreach (var subject in subjects)
            {
                teacher.TeacherSubjects.Add(new TeacherSubject
                {
                    SubjectID = subject.ID
                });
            }

            await _context.SaveChangesAsync();

            return teacher.ID;
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public async Task<int?> DeleteAsync(int ID)
    {
        var teacher = await _context.Teachers
            .Include(t => t.TeacherSubjects)
            .FirstOrDefaultAsync(t => t.ID == ID);
        if (teacher == null)
        {
            return null;
        }
        _context.Teachers.Remove(teacher);
        _context.TeacherSubjects.RemoveRange(teacher.TeacherSubjects);
        await _context.SaveChangesAsync();
        return ID;
    }
}