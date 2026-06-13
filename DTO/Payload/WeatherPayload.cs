using System.Text.Json.Serialization;

namespace DTO.Payload;

public record WeatherPayload
{
    [JsonPropertyName("macAddress")]
    public string MacAddress { get; init; } = string.Empty;
    
    [JsonPropertyName("temperature")]
    public required float Temperature { get; init; }
    
    [JsonPropertyName("pressure")]
    public required float Pressure { get; init; }
    
    [JsonPropertyName("humidity")]
    public required float Humidity { get; init; }
}