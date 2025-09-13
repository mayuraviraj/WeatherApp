using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Interface;

public interface IWeatherService
{
    Task<int> AddForecastAsync(WeatherForecastDto dto);
    
    Task<string> GetTemperatureAsync(string city);
}