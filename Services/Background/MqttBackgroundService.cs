using System.Text;
using System.Text.Json;
using Domain.Models;
using DTO.Payload;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace Services.Background;

public class MqttBackgroundService : BackgroundService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MqttBackgroundService> _logger;
    
    public MqttBackgroundService(
        IConfiguration config,
        IServiceScopeFactory scopeFactory,
        ILogger<MqttBackgroundService> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();
        
        string host = config["Mqtt:Host"] ?? "localhost";
        int port = int.TryParse(config["Mqtt:Port"], out var p) ? p : 1883;
        string user = config["Mqtt:User"] ?? "admin";
        string pass = config["Mqtt:Password"] ?? "twoje_haslo";
        
        _mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(host, port)
            .WithCredentials(user, pass)
            .WithClientId($"DotNetBackend_{Guid.NewGuid()}")
            .WithCleanSession()
            .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessageAsync;

        _mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogInformation("Disconnected from MQTT broker. Trying to reconnect in 5 seconds...");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            await ConnectAndSubscribeAsync(stoppingToken);
        };
        
        await ConnectAndSubscribeAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ConnectAndSubscribeAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
                _logger.LogInformation("Connected to MQTT broker");

                string topicFilter = "sensors/+/weather";
                await _mqttClient.SubscribeAsync(topicFilter);
                _logger.LogInformation($"Subscribed to topic: {topicFilter}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }
    }

    private async Task HandleIncomingMessageAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        string topic = eventArgs.ApplicationMessage.Topic;
        string payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);
        
        _logger.LogInformation($"Received message: {topic}:{payload}");

        using ( var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                var data = JsonSerializer.Deserialize<WeatherPayload>(payload);
                if (data == null) return;

                var sensor = dbContext.Sensors.FirstOrDefault(s => s.MacAddress == data.MacAddress);

                if (sensor == null)
                {
                    _logger.LogInformation($"Sensor not found: {data.MacAddress}");
                    return;
                }

                if (!sensor.IsActive)
                {
                    _logger.LogInformation($"Sensor is not active: {data.MacAddress}");
                    return;
                }

                sensor.LastPingAt = DateTime.UtcNow;

                Measurement measurement = new Measurement
                {
                    SensorId = sensor.Id,
                    Temperature = data.Temperature,
                    Humidity = data.Humidity,
                    Pressure = data.Pressure,
                    Timestamp = DateTime.UtcNow
                };

                dbContext.Measurements.Add(measurement);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"Saved Measurement form sensor: {sensor.MacAddress} at {sensor.LastPingAt}");

            }
            catch (JsonException jex)
            {
                _logger.LogError($"Json parse exception for topic {topic}. {jex.Message}");
            } 
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }
    }
}