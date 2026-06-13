using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    
    public DbSet<Measurement>  Measurements { get; set; }
    public DbSet<Sensor> Sensors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Sensor>()
            .HasMany(s => s.Measurements)
            .WithOne(s => s.Sensor)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Sensor>()
            .HasQueryFilter(s => s.DeletedAt == null);
        
        modelBuilder.Entity<Sensor>()
            .HasIndex(s => s.Topic)
            .IsUnique();

        modelBuilder.Entity<Sensor>()
            .HasIndex(s => s.MacAddress);

        modelBuilder.Entity<Measurement>()
            .HasIndex(m => new
            {
                m.SensorId,
                m.Timestamp
            });
        
        modelBuilder.Entity<Measurement>()
            .HasQueryFilter(m => m.Sensor!.DeletedAt == null);
    }
}