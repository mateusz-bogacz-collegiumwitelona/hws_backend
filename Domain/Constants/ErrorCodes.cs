namespace Domain.Constants;

public static class ErrorCodes
{
    // Ogólne
    public const string InternalError = "INTERNAL_ERROR";
    public const string BadRequest = "BAD_REQUEST";
    public const string NotFound = "NOT_FOUND";
    
    // sensors
    public const string SensorNotFound = "SENSOR_01";
    public const string SensorNotHaveData = "SENSOR_02";
}