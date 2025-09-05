namespace WeatherApp.Domain.Entities;

public class WeatherForecast
{
    public int Id { get; set; }
    public int Temperature { get; set; }
    public string? Summary { get; set; }
}