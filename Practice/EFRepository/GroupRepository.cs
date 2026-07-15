using System.Security.Cryptography.X509Certificates;
using DomainCore;
using DomainCore.DTO;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class GroupRepository : IGroupRepository
{
    private readonly IDbContextFactory<PracticeDbContext> _dbFactory;

    public GroupRepository(IDbContextFactory<PracticeDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }
    public async Task<GroupDto?> GetGroupForNameAsync(string name)
    {
        await using var context = _dbFactory.CreateDbContext();
        return await context.Groups
            .Where(g => g.NameGroup == name)
            .Select(g => new GroupDto
            {
                ID = g.ID,
                NameGroup = g.NameGroup,
                SubjectsName = g.GroupSubjects.Select(s => s.Subject.NameSubjects).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task<List<GroupDto>?> GetAllGroupsAsync()
    {
        await using var context = _dbFactory.CreateDbContext();
        return await context.Groups.Select(g => new GroupDto
            {
                ID = g.ID,
                NameGroup = g.NameGroup,
                SubjectsName = g.GroupSubjects.Select(s => s.Subject.NameSubjects).ToList()
            }).ToListAsync();
    }

    public async Task<int?> CreateAsync(GroupDto groupDto)
    {
        await using var context = _dbFactory.CreateDbContext();
        try
        {
            var group = new Group
            {
                NameGroup = groupDto.NameGroup
            };

            var subjects = await context.Subjects
                .Where(s => groupDto.SubjectsName.Contains(s.NameSubjects))
                .ToListAsync();
            if (subjects.Count != groupDto.SubjectsName.Count)
            {
                return null;
            }
            context.Groups.Add(group);
            foreach (var subject in subjects)
            {
                group.GroupSubjects.Add(new GroupSubject
                {
                    SubjectID = subject.ID
                });
            }
    
            await context.SaveChangesAsync();

            return group.ID;
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public async Task<int?> UpdateAsync(GroupDto groupDto)
    {
        await using var context = _dbFactory.CreateDbContext();
        try
        {
            var group = await context.Groups
                .Include(g => g.GroupSubjects)
                .FirstOrDefaultAsync(g => g.ID == groupDto.ID);
            
            if (group == null)
            {
                return null;
            }

            group.NameGroup = groupDto.NameGroup;

            group.GroupSubjects.Clear();

            var subjects = await context.Subjects
                .Where(s => groupDto.SubjectsName.Contains(s.NameSubjects))
                .ToListAsync();
            if (subjects.Count != groupDto.SubjectsName.Count)
            {
                return null;
            }
            foreach (var subject in subjects)
            {
                group.GroupSubjects.Add(new GroupSubject
                {
                    SubjectID = subject.ID
                });
            }

            await context.SaveChangesAsync();

            return group.ID;
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public async Task<int?> DeleteAsync(int ID)
    {
        await using var context = _dbFactory.CreateDbContext();
        var group = await context.Groups
            .Include(g => g.GroupSubjects) 
            .FirstOrDefaultAsync(g => g.ID == ID);
        if (group == null)
        {
            return null;
        }
        context.Groups.Remove(group);
        context.GroupSubjects.RemoveRange(group.GroupSubjects);
        await context.SaveChangesAsync();
        return ID;
    }
}