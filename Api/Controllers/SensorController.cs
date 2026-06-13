using Api.Controllers.Base;
using Domain.Common;
using DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Api.Controllers;

[ApiController]
public class SensorController : BaseController
{
    private readonly ISensorServices _sensorServices;
    
    public SensorController(ISensorServices sensorServices)
    {
        _sensorServices = sensorServices;
    }
    
    [EndpointSummary("Get newest data send by sensor")]
    [EndpointDescription("Return newest data send by sensor GUID")]
    [ProducesResponseType(typeof(Result<GetSensorMesurmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpGet("{sensorId}")]
    public async Task<IActionResult> GetSensorNewestMesureAsync([FromRoute] Guid sensorId)
    {
        var result = await _sensorServices.GetSensorNewestMesureAsync(sensorId);
        return HandleResult(result);
    }
    
}