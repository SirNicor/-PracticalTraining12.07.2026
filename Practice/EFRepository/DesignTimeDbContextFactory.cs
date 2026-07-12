using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace EFRepository;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PracticeDbContext>{
    
    public PracticeDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(basePath, "..", "WebApplication1"))
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<PracticeDbContext>();
        optionsBuilder.UseNpgsql(connectionString); 
        return new PracticeDbContext(optionsBuilder.Options);
    }
    
}