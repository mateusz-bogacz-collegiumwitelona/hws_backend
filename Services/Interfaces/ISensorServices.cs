using Domain.Common;
using DTO.Request;
using DTO.Response;

namespace Services.Interfaces;

public interface ISensorServices
{
    Task<Result<GetSensorMesurmentResponse>> GetSensorNewestMesureAsync(Guid sensorId, Guid userId);
    Task<Result> AddNewSenorAsync(AddNewSensorRequest request, Guid userId);
    Task<Result> DeleteSensorAsync(Guid sensorId, Guid userId);

    Task<Result<IEnumerable<GetSensorListResponse>>> GetSensorListAsync(Guid userId);
}