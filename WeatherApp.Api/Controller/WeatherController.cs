using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interface;

namespace WeatherApp.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class WeatherController(IWeatherService _service, ILogger<WeatherController> _logger) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult> PostData([FromBody] WeatherForecastDto dto)
    {
        var id = await _service.AddForecastAsync(dto);
        return CreatedAtAction(nameof(PostData), new { id = id }, dto);
    }

    [HttpGet]
    public async Task<ActionResult> GetForecasts()
    {
        _logger.LogInformation("GetForecasts ==================== ");
        string weather = await _service.GetTemperatureAsync("London");
        return Ok(weather);
    }
}