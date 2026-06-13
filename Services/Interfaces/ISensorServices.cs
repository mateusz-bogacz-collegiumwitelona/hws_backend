using Domain.Common;
using DTO.Request;
using DTO.Response;

namespace Services.Interfaces;

public interface ISensorServices
{
    Task<Result<GetSensorMesurmentResponse>> GetSensorNewestMesureAsync(Guid sensorId);
    Task<Result> AddNewSenorAsync(AddNewSensorRequest request);
}