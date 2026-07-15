using EFRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApplication1;

public static class AddedDI
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor(options =>
        {
            options.DetailedErrors = true;
        });
        services.AddDbContextFactory<PracticeDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("EFRepository")));
        services.AddHttpClient();
        services.AddTransient<IClassroomRepository, ClassroomRepository>();
        services.AddTransient<ITeacherRepository, TeacherRepository>();
        services.AddTransient<IGroupRepository, GroupRepository>();
        services.AddTransient<ISubjectRepository, SubjectRepository>();
        services.AddTransient<ILessonRepository, LessonRepository>();
    }
}