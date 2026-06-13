namespace DTO.Response;

public record GetMesurmentsResponse
{
    public required float Temperature { get; init; }
    public required float Humidity { get; init; }
    public required float Pressure { get; init; }
    public required DateTime Timestamp { get; init; }
}