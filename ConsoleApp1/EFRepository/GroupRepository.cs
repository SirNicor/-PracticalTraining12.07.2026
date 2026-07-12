using System.Security.Cryptography.X509Certificates;
using DomainCore;
using DomainCore.DTO;
using Microsoft.EntityFrameworkCore;

namespace EFRepository;

public class GroupRepository : IGroupRepository
{
    private readonly PracticeDbContext _context;

    public GroupRepository(PracticeDbContext context)
    {
        _context = context;
    }
    public async Task<GroupDto?> GetGroupForNameAsync(string name)
    {
        return await _context.Groups
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
        return await _context.Groups.Select(g => new GroupDto
            {
                ID = g.ID,
                NameGroup = g.NameGroup,
                SubjectsName = g.GroupSubjects.Select(s => s.Subject.NameSubjects).ToList()
            }).ToListAsync();
    }

    public async Task<int?> CreateAsync(GroupDto groupDto)
    {
        try
        {
            var group = new Group
            {
                NameGroup = groupDto.NameGroup
            };

            var subjects = await _context.Subjects
                .Where(s => groupDto.SubjectsName.Contains(s.NameSubjects))
                .ToListAsync();
            if (subjects.Count != groupDto.SubjectsName.Count)
            {
                return null;
            }
            _context.Groups.Add(group);
            foreach (var subject in subjects)
            {
                group.GroupSubjects.Add(new GroupSubject
                {
                    SubjectID = subject.ID
                });
            }
    
            await _context.SaveChangesAsync();

            return group.ID;
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public async Task<int?> UpdateAsync(GroupDto groupDto)
    {
        try
        {
            var group = await _context.Groups
                .Include(g => g.GroupSubjects)
                .FirstOrDefaultAsync(g => g.ID == groupDto.ID);
            
            if (group == null)
            {
                return null;
            }

            group.NameGroup = groupDto.NameGroup;

            group.GroupSubjects.Clear();

            var subjects = await _context.Subjects
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

            await _context.SaveChangesAsync();

            return group.ID;
        }
        catch (DbUpdateException)
        {
            return null;
        }
    }

    public async Task<int?> DeleteAsync(int ID)
    {
        var group = await _context.Groups
            .Include(g => g.GroupSubjects) 
            .FirstOrDefaultAsync(g => g.ID == ID);
        if (group == null)
        {
            return null;
        }
        _context.Groups.Remove(group);
        _context.GroupSubjects.RemoveRange(group.GroupSubjects);
        await _context.SaveChangesAsync();
        return ID;
    }
}