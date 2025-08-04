using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;

namespace Pi.GlobalMarketData.MigrationProxy.Controllers;

[ApiController]
[Route("weather-forecast")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Get weather forecast list
    /// </summary>
    /// <returns>Summaries</returns>
    [HttpGet(Name = "GetWeatherForecasts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string[]>))]
    public async Task<IActionResult> Get()
    {
        var result = await Task.FromResult(Summaries);
        return Ok(new ApiResponse<string[]>(result));
    }

    /// <summary>
    ///     Get weather forecast by x
    /// </summary>
    /// <returns>One summary</returns>
    [HttpGet("{index}", Name = "GetWeatherForecast")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Get([FromRoute] int index, [FromQuery] bool error)
    {
        if (error)
            return Problem(statusCode: StatusCodes.Status409Conflict, detail: "Something wrong", title: "Error code");

        try
        {
            var result = await Task.FromResult(Summaries);
            return Ok(new ApiResponse<string>(result[index]));
        }
        catch (IndexOutOfRangeException)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Can't find value",
                title: "Some error code");
        }
    }
}