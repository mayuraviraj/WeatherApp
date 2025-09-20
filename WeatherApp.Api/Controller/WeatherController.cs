using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interface;

namespace WeatherApp.Api.Controller;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class WeatherController(IWeatherService _service, ILogger<WeatherController> _logger) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<WeatherForecaseReponse>> PostData([FromBody] WeatherForecastDto dto)
    {
        var id = await _service.AddForecastAsync(dto);
        var weatherForecaset = new WeatherForecaseReponse
        {
            Temperature = id,
            Summary = dto.Summary,
        };
        return CreatedAtAction(nameof(PostData), new { id = id }, weatherForecaset);
    }

    [HttpGet]
    public async Task<ActionResult> GetForecasts()
    {
        _logger.LogInformation("GetForecasts ==================== ");
        string weather = "TEST";// await _service.GetTemperatureAsync("London");
        var dto = new WeatherForecastDto
        {
            Summary = "TEST",
            Temperature = 10,
        };
        var id = await _service.AddForecastAsync(dto);
        return Ok(weather);
    }
}