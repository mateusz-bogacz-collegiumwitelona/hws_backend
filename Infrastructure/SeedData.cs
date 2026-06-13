using Domain.Models;

namespace Infrastructure;

public static class SeedData
{
    public static void Seed(AppDbContext context)
    {
        if (context.Sensors.Any()) return;

        var esp = new Sensor()
        {
            Name = "Test ESP with BME280",
            Location = "Obok mnie",
            MacAddress = "68:25:DD:21:3D:B8",
            Topic = "sensors/68:25:DD:21:3D:B8/weather",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        context.Sensors.Add(esp);
        context.SaveChanges();
    }
}