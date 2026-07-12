using DomainCore;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class SubjectRepository : ISubjectRepository
{
    private readonly PracticeDbContext _context;

    public SubjectRepository(PracticeDbContext context)
    {
        _context = context;
    }

    public async Task<Subject?> GetSubjectsForNameAsync(string name)
    {
        var subject = await _context.Subjects.Where(p => p.NameSubjects == name)
            .FirstOrDefaultAsync();
        return subject;
    }

    public async Task<List<Subject>?> GetAllSubjectsAsync()
    {
        var subjects = await _context.Subjects.ToListAsync();
        return subjects;
    }

    public async Task<int?> CreateAsync(Subject subject)
    {
        try
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return null;
        }
        return subject.ID;
    }

    public async Task<int?> UpdateAsync(Subject subject)
    {
        try
        {
            var trackedEntity = _context.Subjects.Local.FirstOrDefault(s => s.ID == subject.ID);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return null;
        }
        return subject.ID;
    }

    public async Task<int?> DeleteAsync(int ID)
    {
        var subject = await _context.Subjects.FindAsync(ID);
        if (subject == null)
        {
            return null;
        }
        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return ID;
    }
}