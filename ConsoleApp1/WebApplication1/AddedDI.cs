using EFRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApplication1;

public static class AddedDI
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PracticeDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("EFRepository")));
        services.AddScoped<IClassroomRepository, ClassroomRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
    }
}