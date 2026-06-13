using Api.Controllers.Base;
using Domain.Common;
using DTO.Request;
using DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorController : AuthControllerBase
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
        var result = await _sensorServices.GetSensorNewestMesureAsync(sensorId, CurrentUserId);
        return HandleResult(result);
    }

    [EndpointSummary("Add new sensor")]
    [EndpointDescription("This make exacly what is in title")]
    [ProducesResponseType(typeof(Result<GetSensorMesurmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpPut("")]
    public async Task<IActionResult> AddNewSenorAsync([FromBody] AddNewSensorRequest request)
    {
        var result = await _sensorServices.AddNewSenorAsync(request, CurrentUserId);
        
        return HandleResult(result);
    }
    
    [EndpointSummary("Delete sensor")]
    [EndpointDescription("This make exacly what is in title")]
    [ProducesResponseType(typeof(Result<GetSensorMesurmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteSensorAsync([FromQuery]Guid sensorId)
    {
        var  result = await _sensorServices.DeleteSensorAsync(sensorId, CurrentUserId);
        return HandleResult(result);
    }
    
    [EndpointSummary("Get  sensor list")]
    [EndpointDescription("This make exacly what is in title")]
    [ProducesResponseType(typeof(Result<GetSensorMesurmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpGet("list")]
    public async Task<IActionResult> GetSensorListAsync()
    {
        var result = await _sensorServices.GetSensorListAsync(CurrentUserId);
        return HandleResult(result);
    }
    
    
    [EndpointSummary("Get paginated sensor measurements")]
    [EndpointDescription("Returns a paginated list of measurements for a specific sensor.")]
    [ProducesResponseType(typeof(Result<GetSensorMesurmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpGet("measurments/{sensorId}")]
    public async Task<IActionResult> GetSensorAllMesurments(
        [FromRoute] Guid sensorId,
        [FromQuery] PagedRequest paged
    )
    {
        var result = await _sensorServices.GetSensorAllMesurments(
            CurrentUserId,
            sensorId,
            paged
        );
        
        return HandleResult(result);
    }
    
}