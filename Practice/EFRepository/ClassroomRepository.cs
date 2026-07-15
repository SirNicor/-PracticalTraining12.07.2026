using DomainCore;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class ClassroomRepository : IClassroomRepository
{
    private readonly IDbContextFactory<PracticeDbContext> _dbFactory;

    public ClassroomRepository(IDbContextFactory<PracticeDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }
    public async Task<Classroom?> GetClassroomForNumberAsync(string number)
    {
        await using var context = _dbFactory.CreateDbContext();
        var classroom = await context.Classrooms.Where(p => p.NumberClassroom == number)
            .FirstOrDefaultAsync();
        return classroom;
    }

    public async Task<List<Classroom>?> GetAllClassroomsAsync()
    {
        await using var context = _dbFactory.CreateDbContext();
        var classroom = await context.Classrooms.ToListAsync();
        return classroom;
    }

    public async Task<int?> CreateAsync(Classroom classroom)
    {
        await using var context = _dbFactory.CreateDbContext();
        try
        {
            context.Classrooms.Add(classroom);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return null;
        }
        return classroom.ID;
    }

    public async Task<int?> UpdateAsync(Classroom classroom)
    {
        await using var context = _dbFactory.CreateDbContext();
        try
        {
            var trackedEntity = context.Classrooms.Local.FirstOrDefault(s => s.ID == classroom.ID);
            if (trackedEntity != null)
            {
                context.Entry(trackedEntity).State = EntityState.Detached;
            }
            context.Classrooms.Update(classroom);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return null;
        }
        return classroom.ID;
    }

    public async Task<int?> DeleteAsync(int id)
    {
        await using var context = _dbFactory.CreateDbContext();
        var classroom = await context.Classrooms.FindAsync(id);
        if (classroom == null)
        {
            return null;
        }
        context.Classrooms.Remove(classroom);
        await context.SaveChangesAsync();
        return id;
    }
}
