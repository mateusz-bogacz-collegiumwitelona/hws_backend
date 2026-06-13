using Domain.Common;
using Domain.Constants;
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

    public async Task<Result<GetSensorMesurmentResponse>> GetSensorNewestMesureAsync(Guid sensorId)
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
}