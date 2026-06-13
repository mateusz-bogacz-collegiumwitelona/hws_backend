namespace DTO.Request;

public record EditSensorRequest
{
    public required Guid SensorId { get; init; }
    public string? Name { get; init; }
    public string? Location { get; init; }
    public int? Type { get; init; }
    public bool? IsActive { get; init; }
}