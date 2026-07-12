using DomainCore;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class ClassroomRepository : IClassroomRepository
{
    private readonly PracticeDbContext _context;

    public ClassroomRepository(PracticeDbContext context)
    {
        _context = context;
    }
    public async Task<Classroom?> GetClassroomForNumberAsync(string number)
    {
        var classroom = await _context.Classrooms.Where(p => p.NumberClassroom == number)
            .FirstOrDefaultAsync();
        return classroom;
    }

    public async Task<List<Classroom>?> GetAllClassroomsAsync()
    {
        var classroom = await _context.Classrooms.ToListAsync();
        return classroom;
    }

    public async Task<int?> CreateAsync(Classroom classroom)
    {
        try
        {
            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return null;
        }
        return classroom.ID;
    }

    public async Task<int?> UpdateAsync(Classroom classroom)
    {
        try
        {
            var trackedEntity = _context.Classrooms.Local.FirstOrDefault(s => s.ID == classroom.ID);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }
            _context.Classrooms.Update(classroom);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return null;
        }
        return classroom.ID;
    }

    public async Task<int?> DeleteAsync(int id)
    {
        var classroom = await _context.Classrooms.FindAsync(id);
        if (classroom == null)
        {
            return null;
        }
        _context.Classrooms.Remove(classroom);
        await _context.SaveChangesAsync();
        return id;
    }
}
