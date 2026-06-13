using Domain.Common;
using Domain.Constants;
using Domain.Enums;
using Domain.Models;
using DTO.Request;
using DTO.Response;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Services.Interfaces;

namespace Services.Services;

public class SensorServices : ISensorServices
{
    private readonly AppDbContext _context;
    
    public SensorServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetSensorMesurmentResponse>> GetSensorNewestMesureAsync(Guid sensorId, Guid userId)
    {
        try
        {
            if (!await _context.Sensors.AnyAsync(s => s.Id == sensorId))
                return Result<GetSensorMesurmentResponse>.Failure(
                    message: "Sensor not found",
                    statusCode: StatusCodes.Status404NotFound,
                    errorCode: ErrorCodes.SensorNotFound
                );

            var latestMesurments = await _context.Measurements
                .AsNoTracking()
                .Include(m => m.Sensor)
                .Where(m => m.SensorId == sensorId)
                .Where(m => m.Sensor.UserId ==  userId)
                .OrderByDescending(m => m.Timestamp)
                .Select(m => new GetSensorMesurmentResponse
                {
                    Temperature = m.Temperature,
                    Humidity = m.Humidity,
                    Pressure = m.Pressure,
                    Timestamp = m.Timestamp
                })
                .FirstOrDefaultAsync();

            if (latestMesurments == null)
            {
                return Result<GetSensorMesurmentResponse>.Failure(
                    message: "Sensor not have data",
                    statusCode: StatusCodes.Status404NotFound,
                    errorCode: ErrorCodes.SensorNotHaveData
                );
            }

            return Result<GetSensorMesurmentResponse>.Success(
                data: latestMesurments,
                message: "Sensor data found",
                statusCode: StatusCodes.Status200OK
            );
        }
        catch (Exception ex)
        {
            return Result<GetSensorMesurmentResponse>.Failure(
                "An error occurred while get sensor data",
                ErrorCodes.InternalError,
                StatusCodes.Status500InternalServerError,
                new List<string> { ex.Message }
            );
        }
    }

    public async Task<Result> AddNewSenorAsync(AddNewSensorRequest request, Guid userId)
    {
        try
        {
            if (await _context.Sensors.AnyAsync(s => s.MacAddress == request.MacAddress))
            {
                return Result.Failure(
                    message: "Sensor with mac address already exists",
                    statusCode: StatusCodes.Status409Conflict,
                    errorCode: ErrorCodes.SensorAlreadyExist
                );
            }
            
            Sensor sensor = new Sensor
            {
                Type = GetType(request.Type),
                Name = request.Name,
                Location = request.Location,
                Topic =  GetTopic(GetType(request.Type), request.MacAddress),
                MacAddress = request.MacAddress,
                UserId = userId
            };
            
            _context.Sensors.Add(sensor);
            await _context.SaveChangesAsync();
            
            return Result.Success(
                message: "Sensor added",
                statusCode: StatusCodes.Status201Created
                );

        }
        catch (Exception ex)
        {
            return Result.Failure(
                "An error occurred while add sensor",
                ErrorCodes.InternalError,
                StatusCodes.Status500InternalServerError,
                new List<string> { ex.Message }
            );
        }
    }

    public async Task<Result> DeleteSensorAsync(Guid sensorId, Guid userId)
    {
        try
        {
            var sensor = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == sensorId && s.UserId == userId);

            if (sensor == null)
            {
                return Result.Failure(
                    message: "Sensor not found",
                    statusCode: StatusCodes.Status404NotFound,
                    errorCode: ErrorCodes.SensorNotFound
                );
            }

            _context.Remove(sensor);
            await _context.SaveChangesAsync();
            
            return Result.Success(
                message: "Sensor deleted",
                statusCode: StatusCodes.Status200OK
            );
        }
        catch (Exception ex)
        {
            return Result.Failure(
                "An error occurred while remove sensor",
                ErrorCodes.InternalError,
                StatusCodes.Status500InternalServerError,
                new List<string> { ex.Message }
            );
        }
    }
    
    public async Task<Result<IEnumerable<GetSensorListResponse>>> GetSensorListAsync(Guid userId)
    {
        try
        {
            var sensors = await _context.Sensors
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Select(s => new GetSensorListResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Location = s.Location,
                    Type = s.Type.ToString(),
                    MacAddress = s.MacAddress,
                    IsActive = s.IsActive,
                    LastPingAt = s.LastPingAt,
                }).ToListAsync();

            return Result<IEnumerable<GetSensorListResponse>>.Success(
                data: sensors,
                message: "Sensors retrieved successfully",
                statusCode: StatusCodes.Status200OK
            );
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<GetSensorListResponse>>.Failure(
                message: "An error occurred while retrieving sensors",
                errorCode: ErrorCodes.InternalError,
                statusCode: StatusCodes.Status500InternalServerError,
                errors: new List<string> {ex.Message}
            );
        }
    }
    private static SensorTypeEnum GetType(int sensorType)
    {
        return sensorType switch
        {
            1 => SensorTypeEnum.WeatherStation,
            _ => throw new ArgumentOutOfRangeException(nameof(sensorType), $"Unknown sensor type: {sensorType}")
        };
    }
    
    private static string GetTopic(SensorTypeEnum type, string macAddress)
    {
        return type switch
        {
            SensorTypeEnum.WeatherStation => $"sensors/{macAddress}/weather",
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"No topic mapping for sensor type: {type}")
        };
    }
}