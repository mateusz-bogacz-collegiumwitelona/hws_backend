namespace Domain.Models;

public class Measurement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid SensorId { get; set; }
    public Sensor? Sensor { get; set; }
    
    public required float Temperature { get; set; }
    public required float Humidity { get; set; }
    public required float Pressure { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}