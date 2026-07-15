    using DomainCore;
    using DomainCore.DTO;
    using Microsoft.EntityFrameworkCore;

    namespace EFRepository;

    public class TeacherRepository : ITeacherRepository
    {
        private readonly IDbContextFactory<PracticeDbContext> _dbFactory;

        public TeacherRepository(IDbContextFactory<PracticeDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<TeacherDto?> GetTeacherForNameAsync(string name)
        {
            await using var context = _dbFactory.CreateDbContext();
            return await context.Teachers
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
            await using var context = _dbFactory.CreateDbContext();
            return await context.Teachers.Select(t => new TeacherDto
            {
                ID = t.ID,
                FIO = t.FIO,
                SubjectsName = t.TeacherSubjects.Select(ts => ts.Subject.NameSubjects).ToList()
            }).ToListAsync();
        }

        public async Task<int?> CreateAsync(TeacherDto teacherDto)
        {
            await using var context = _dbFactory.CreateDbContext();
            try
            {
                var teacher = new Teacher
                {
                    FIO = teacherDto.FIO
                };

                var subjects = await context.Subjects
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
                
                context.Teachers.Add(teacher);
                await context.SaveChangesAsync();

                return teacher.ID;
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }

        public async Task<int?> UpdateAsync(TeacherDto teacherDto)
        {
            await using var context = _dbFactory.CreateDbContext();
            try
            {
                var teacher = await context.Teachers
                    .Include(t => t.TeacherSubjects)
                    .FirstOrDefaultAsync(t => t.ID == teacherDto.ID);

                if (teacher == null)
                {
                    return null;
                }

                teacher.FIO = teacherDto.FIO;

                teacher.TeacherSubjects.Clear();

                var subjects = await context.Subjects
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

                await context.SaveChangesAsync();

                return teacher.ID;
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }

        public async Task<int?> DeleteAsync(int ID)
        {
            await using var context = _dbFactory.CreateDbContext();
            var teacher = await context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.ID == ID);
            if (teacher == null)
            {
                return null;
            }
            context.Teachers.Remove(teacher);
            context.TeacherSubjects.RemoveRange(teacher.TeacherSubjects);
            await context.SaveChangesAsync();
            return ID;
        }
    }