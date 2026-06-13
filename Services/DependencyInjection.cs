using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Background;
using Services.Interfaces;
using Services.Services;

namespace Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<MqttBackgroundService>();
        services.AddScoped<ISensorServices, SensorServices>();
        
        return services;
    }
}