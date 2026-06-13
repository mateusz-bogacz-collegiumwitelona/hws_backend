using Domain.Common;
using DTO.Response;

namespace Services.Interfaces;

public interface ISensorServices
{
    Task<Result<GetSensorMesurmentResponse>> GetSensorNewestMesureAsync(Guid sensorId);
}