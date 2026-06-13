using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure;

public class SeedData
{
    private readonly UserManager<User> _userManager;

    public SeedData(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task Seed(AppDbContext context)
    {
       await SeedUser(context);
       await SeedSensor(context);
    }

    private async Task SeedUser(AppDbContext context)
    {
        string userName = "test";
        string email = "test@example.pl";
        
        if (context.Users.Any(x => x.UserName == userName)) return;

        User user = new User
        {
            Email = email,
            NormalizedEmail = email.ToUpper(),
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        
        await _userManager.CreateAsync(user, "Test123!");
    }

    private async Task SeedSensor(AppDbContext context)
    {
        if (context.Sensors.Any()) return;
        
        User? user = await _userManager.FindByNameAsync("test");
        
        if (user == null) return;
        
        var esp = new Sensor()
        {
            Name = "Test ESP with BME280",
            Location = "Obok mnie",
            MacAddress = "68:25:DD:21:3D:B8",
            Topic = "sensors/68:25:DD:21:3D:B8/",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Type = SensorTypeEnum.WeatherStation,
            UserId =  user.Id
        };
        
        context.Sensors.Add(esp);
        await context.SaveChangesAsync();
    }
}