namespace Domain.Constants;

public static class ErrorCodes
{
    // Ogólne
    public const string InternalError = "INTERNAL_ERROR";
    public const string BadRequest = "BAD_REQUEST";
    public const string NotFound = "NOT_FOUND";
    public const string InvalidCredentials = "AUTH_003";
    public const string UnauthorizedAccess = "UNAUTHORIZED_ACCESS";
    
    // sensors
    public const string SensorNotFound = "SENSOR_01";
    public const string SensorNotHaveData = "SENSOR_02";
    public const string SensorAlreadyExist = "SENSOR_03";
    public const string SensorTypeNotFound = "SENSOR_04";
}