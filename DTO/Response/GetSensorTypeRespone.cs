namespace DTO.Response;

public record GetSensorTypeRespone
{
    public required string Name { get; init; }
    public required int Number { get; init; }

};