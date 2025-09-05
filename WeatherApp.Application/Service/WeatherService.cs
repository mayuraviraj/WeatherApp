using Microsoft.Extensions.Logging;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interface;
using WeatherApp.Domain.Entities;
using WeatherApp.Persistence.Data;

namespace WeatherApp.Application.Service;

public class WeatherService(AppDbContext  _context, IHttpClientFactory  _httpClientFactory, 
    ILogger<WeatherService> _logger) : IWeatherService
{
    public async Task<int> AddForecastAsync(WeatherForecastDto dto)
    {
        var enity = new WeatherForecast
        {
            Temperature = dto.Temperature,
            Summary = dto.Summary,
        };
        _context.WeatherForecasts.Add(enity);
        await _context.SaveChangesAsync();
        return enity.Id;
    }

    public async Task<string> GetTemperatureAsync(long latitude, long longitude)
    {
        _logger.LogInformation("Calling open meteo api with {latitude} and {longitude}",
            latitude, longitude);
        var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m";
        var client = _httpClientFactory.CreateClient("OpenMeteo");
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        return null;
    }
}