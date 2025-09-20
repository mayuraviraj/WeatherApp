using Microsoft.Extensions.Logging;
using Polly.Registry;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interface;
using WeatherApp.Domain.Entities;
using WeatherApp.Persistence.Data;

namespace WeatherApp.Application.Service;

public class WeatherService(AppDbContext  _context, IHttpClientFactory  _httpClientFactory, 
    ResiliencePipelineProvider<string> pipelineProvider,
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

    public async Task<string> GetTemperatureAsync(string city)
    {
        var weatherForecast = new WeatherForecast
        {
            Id = 1,
            Summary = "test",
            Temperature = 23
        };
        _context.WeatherForecasts.Add(weatherForecast);
        var id = await _context.SaveChangesAsync();
        
        
        _logger.LogInformation("Saved {}", id);

        var pipeline = pipelineProvider.GetPipeline("default");
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=661a1bcf2fae84fe5e5298071369b64c&units=metric";
        var client = _httpClientFactory.CreateClient("OpenMeteoClient");
        
        var response = await pipeline.ExecuteAsync(async ct => await client.GetAsync(url, ct));
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
        
        // try
        // {
        //     var response = await client.GetAsync(url);
        //     response.EnsureSuccessStatusCode();
        //     var content = await response.Content.ReadAsStringAsync();
        //     return content;
        // }
        // catch (HttpRequestException ex)
        // {
        //     _logger.LogError(ex, ex.Message);
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, ex.Message);
        // }
        // return null;
    }
}