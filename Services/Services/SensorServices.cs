using Domain.Common;
using Domain.Constants;
using Domain.Enums;
using Domain.Models;
using DTO.Request;
using DTO.Response;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Services.Helpers;
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
        if (!await _context.Sensors.AnyAsync(s => s.Id == sensorId && s.UserId == userId))
        {
            return Result<GetSensorMesurmentResponse>.Failure(
                message: "Sensor not found or access denied",
                statusCode: StatusCodes.Status404NotFound,
                errorCode: ErrorCodes.SensorNotFound
            );
        }

        var latestMesurments = await _context.Measurements
            .AsNoTracking()
            .Where(m => m.SensorId == sensorId)
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
                message: "Sensor does not have data",
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

    public async Task<Result> AddNewSenorAsync(AddNewSensorRequest request, Guid userId)
    {
        if (await _context.Sensors.AnyAsync(s => s.MacAddress == request.MacAddress))
        {
            return Result.Failure(
                message: "Sensor with this MAC address already exists",
                statusCode: StatusCodes.Status409Conflict,
                errorCode: ErrorCodes.SensorAlreadyExist
            );
        }
        
        Sensor sensor = new Sensor
        {
            Type = GetType(request.Type),
            Name = request.Name,
            Location = request.Location,
            Topic = GetTopic(GetType(request.Type), request.MacAddress),
            MacAddress = request.MacAddress,
            UserId = userId
        };
        
        _context.Sensors.Add(sensor);
        await _context.SaveChangesAsync();
        
        return Result.Success("Sensor added", StatusCodes.Status201Created);
    }

    public async Task<Result> DeleteSensorAsync(Guid sensorId, Guid userId)
    {
        var sensor = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == sensorId && s.UserId == userId);

        if (sensor == null)
        {
            return Result.Failure(
                message: "Sensor not found or access denied",
                statusCode: StatusCodes.Status404NotFound,
                errorCode: ErrorCodes.SensorNotFound
            );
        }

        _context.Remove(sensor);
        await _context.SaveChangesAsync();
        
        return Result.Success("Sensor deleted", StatusCodes.Status200OK);
    }
    
    public async Task<Result<IEnumerable<GetSensorListResponse>>> GetSensorListAsync(Guid userId)
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

    public async Task<Result<PagedResult<GetMesurmentsResponse>>> GetSensorAllMesurments(
        Guid userId, 
        Guid sensorId, 
        PagedRequest paged
    )
    {
        if (!await _context.Sensors.AnyAsync(s => s.Id == sensorId && s.UserId == userId))
        {
            return Result<PagedResult<GetMesurmentsResponse>>.Failure(
                message: "Sensor not found",
                statusCode: StatusCodes.Status404NotFound,
                errorCode: ErrorCodes.SensorNotFound
            );
        }

        var queryTask = _context.Measurements
            .AsNoTracking()
            .Where(s => s.SensorId == sensorId)
            .OrderByDescending(m => m.Timestamp)
            .Select(m => new GetMesurmentsResponse
            {
                Temperature = m.Temperature,
                Humidity = m.Humidity,
                Pressure = m.Pressure,
                Timestamp = m.Timestamp,
            }).ToListAsync(); 

        return await queryTask.ToPagedResultAsync(paged);
    }

    public async Task<Result<GetMesurmentsResponse>> GetSensorAveragesMesurments(Guid userId, Guid sensorId)
    {
        if (!await _context.Sensors.AnyAsync(s => s.Id == sensorId && s.UserId == userId))
        {
            return Result<GetMesurmentsResponse>.Failure(
                message: "Sensor not found",
                statusCode: StatusCodes.Status404NotFound,
                errorCode: ErrorCodes.SensorNotFound
            );
        }

        var response = await _context.Measurements
            .AsNoTracking()
            .Where(s => s.SensorId == sensorId)
            .GroupBy(m => 1)
            .Select(g => new GetMesurmentsResponse
            {
                Temperature = (float)Math.Round(g.Average(m => m.Temperature), 2),
                Humidity = (float)Math.Round(g.Average(m => m.Humidity), 2),
                Pressure = (float)Math.Round(g.Average(m => m.Pressure), 2),
                Timestamp = DateTime.UtcNow
            }).FirstOrDefaultAsync();

        if (response == null)
        {
            return Result<GetMesurmentsResponse>.Failure(
                message: "Not enough data to calculate averages",
                statusCode: StatusCodes.Status404NotFound,
                errorCode: ErrorCodes.SensorNotFound
            );
        }

        return Result<GetMesurmentsResponse>.Success(
            data: response,
            message: "Averages calculated successfully",
            statusCode: StatusCodes.Status200OK
        );
    }

    public async Task<Result> EditSensorAsync(Guid userId, EditSensorRequest request)
    {
        Sensor sensor = await _context.Sensors.FirstOrDefaultAsync(s => s.Id == request.SensorId && s.UserId == userId);

        if (sensor == null)
        {
            return Result.Failure(
                message: "Sensor not found or access denied",
                statusCode: StatusCodes.Status404NotFound,
                errorCode: ErrorCodes.SensorNotFound
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            sensor.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            sensor.Location = request.Location;
        }

        if (request.IsActive.HasValue)
        {
            sensor.IsActive = request.IsActive.Value;
        }

        if (request.Type.HasValue)
        {
            int type = (int)request.Type;
            sensor.Type = GetType(type);
        }

        _context.Sensors.Update(sensor);
        await _context.SaveChangesAsync();
        
        return Result.Success("Edited sensor successfully", StatusCodes.Status200OK);
    }

    public async Task<Result<List<GetSensorTypeRespone>>> GetSensorTypeAsync()
    {
        var respone = Enum.GetValues<SensorTypeEnum>()
            .Select(s => new GetSensorTypeRespone
            {
                Name = s.ToString(),
                Number = (int)s
            }).ToList();

        return Result<List<GetSensorTypeRespone>>.Success(
            data: respone,
            message: "Sensor type list retrieved successfully",
            statusCode: StatusCodes.Status200OK);
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