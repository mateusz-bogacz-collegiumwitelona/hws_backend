namespace DTO.Response;

public record GetSensorListResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Location { get; init; }
    public required string Type { get; init; }
    public string? MacAddress { get; init; }
    public required bool IsActive { get; init; }
    public DateTime? LastPingAt { get; init; }
}