using Domain.Enums;

namespace Domain.Models;

public class Sensor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public SensorTypeEnum Type { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Topic { get; set; }
    public string? MacAddress { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? LastPingAt { get; set; }
    
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}