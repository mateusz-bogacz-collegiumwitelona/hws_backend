using System.ComponentModel.DataAnnotations;

namespace DTO.Request;

public record AddNewSensorRequest
{
    [Required(ErrorMessage = "Sensor mac address is required")]
    [RegularExpression(
        @"^([0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2}$",
        ErrorMessage = "Sensor mac address is invalid")]
    public string MacAddress { get; init; }
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Sensor name is required")]
    [Length(minimumLength: 3, maximumLength:30, ErrorMessage = "Name must be between 3 and 30 characters")]
    public string Name { get; init; }
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Sensor location is required")]
    [Length(minimumLength: 3, maximumLength:30, ErrorMessage = "Location name must be between 3 and 30 characters")]
    public string Location { get; init; }
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "Sensor type is required")]
    public int Type { get; init; }
}