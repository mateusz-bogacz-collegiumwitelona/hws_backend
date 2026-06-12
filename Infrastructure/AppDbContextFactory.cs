using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables() 
            .Build();

        string connectionString = configuration["ConnectionStrings:DefaultConnection"] 
                                  ?? "Host=localhost;Database=postgres;Username=postgres;Password=postgres;Port=5432";
        
        builder.UseNpgsql(connectionString);
        
        return new AppDbContext(builder.Options);
    }
}