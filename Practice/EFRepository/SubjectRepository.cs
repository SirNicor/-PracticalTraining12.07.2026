using DomainCore;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class SubjectRepository : ISubjectRepository
{
    private readonly IDbContextFactory<PracticeDbContext> _dbFactory;

    public SubjectRepository(IDbContextFactory<PracticeDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<Subject?> GetSubjectsForNameAsync(string name)
    {
        await using var context = _dbFactory.CreateDbContext();
        var subject = await context.Subjects.Where(p => p.NameSubjects == name)
            .FirstOrDefaultAsync();
        return subject;
    }

    public async Task<List<Subject>?> GetAllSubjectsAsync()
    {
        await using var context = _dbFactory.CreateDbContext();
        var subjects = await context.Subjects.ToListAsync();
        return subjects;
    }

    public async Task<int?> CreateAsync(Subject subject)
    {
        await using var context = _dbFactory.CreateDbContext();
        try
        {
            context.Subjects.Add(subject);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return null;
        }
        return subject.ID;
    }

    public async Task<int?> UpdateAsync(Subject subject)
    {
        await using var context = _dbFactory.CreateDbContext();
        try
        {
            var trackedEntity = context.Subjects.Local.FirstOrDefault(s => s.ID == subject.ID);
            if (trackedEntity != null)
            {
                context.Entry(trackedEntity).State = EntityState.Detached;
            }
            context.Subjects.Update(subject);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return null;
        }
        return subject.ID;
    }

    public async Task<int?> DeleteAsync(int ID)
    {
        await using var context = _dbFactory.CreateDbContext();
        var subject = await context.Subjects.FindAsync(ID);
        if (subject == null)
        {
            return null;
        }
        context.Subjects.Remove(subject);
        await context.SaveChangesAsync();
        return ID;
    }
}