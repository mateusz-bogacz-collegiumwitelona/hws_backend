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
    
    [EndpointSummary("Get newest data sent by sensor")]
    [EndpointDescription("Returns the most recent measurement data sent by a specific sensor.")]
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
    [EndpointDescription("Registers a new sensor assigned to the currently authenticated user.")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpPost("")]
    public async Task<IActionResult> AddNewSenorAsync([FromBody] AddNewSensorRequest request)
    {
        var result = await _sensorServices.AddNewSenorAsync(request, CurrentUserId);
        return HandleResult(result);
    }
    
    [EndpointSummary("Delete sensor")]
    [EndpointDescription("Permanently removes a sensor and all its associated data.")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{sensorId}")]
    public async Task<IActionResult> DeleteSensorAsync([FromRoute] Guid sensorId)
    {
        var result = await _sensorServices.DeleteSensorAsync(sensorId, CurrentUserId);
        return HandleResult(result);
    }
    
    [EndpointSummary("Get sensor list")]
    [EndpointDescription("Retrieves a list of all sensors owned by the authenticated user.")]
    [ProducesResponseType(typeof(Result<IEnumerable<GetSensorListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpGet("list")]
    public async Task<IActionResult> GetSensorListAsync()
    {
        var result = await _sensorServices.GetSensorListAsync(CurrentUserId);
        return HandleResult(result);
    }
    
    [EndpointSummary("Get paginated sensor measurements")]
    [EndpointDescription("Returns a paginated list of historical measurements for a specific sensor.")]
    [ProducesResponseType(typeof(Result<PagedResult<GetMesurmentsResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpGet("measurements/{sensorId}")] 
    public async Task<IActionResult> GetSensorAllMesurments(
        [FromRoute] Guid sensorId,
        [FromQuery] PagedRequest paged
    )
    {
        var result = await _sensorServices.GetSensorAllMesurments(CurrentUserId, sensorId, paged);
        return HandleResult(result);
    }
    
    [EndpointSummary("Get average sensor measurements")]
    [EndpointDescription("Returns the average measurement values for a specific sensor.")]
    [ProducesResponseType(typeof(Result<GetMesurmentsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpGet("measurements/{sensorId}/averages")]
    public async Task<IActionResult> GetSensorAveragesMesurments(
        [FromRoute] Guid sensorId
    )
    {
        var result = await _sensorServices.GetSensorAveragesMesurments(CurrentUserId, sensorId);
        return HandleResult(result);
    }

    [EndpointSummary("Edit sensor data")]
    [EndpointDescription("Updates specific properties of an existing sensor.")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object>), StatusCodes.Status500InternalServerError)]
    [HttpPatch("edit")]
    public async Task<IActionResult> EditSensorAsync([FromBody] EditSensorRequest request)
    {
        var response = await _sensorServices.EditSensorAsync(CurrentUserId, request);
        return HandleResult(response);
    }
}