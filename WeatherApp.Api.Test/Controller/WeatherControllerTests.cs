
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApp.Api.Controller;
using WeatherApp.Application.DTOs;
using WeatherApp.Application.Interface;
using NUnit.Framework;

namespace WeatherApp.Api.Test.Controller;

[TestFixture]
public class WeatherControllerTests
{
    private Mock<IWeatherService> _weatherService;
    private Mock<ILogger<WeatherController>> _logger;
    private WeatherController _controller;

    [SetUp]
    public void Setup()
    {
        _weatherService = new Mock<IWeatherService>();
        _logger = new Mock<ILogger<WeatherController>>();
        _controller = new WeatherController(_weatherService.Object, _logger.Object);
    }

    [Test]
    public async Task PostData_ReturnCreatedAtAction()
    {
        var dto = new WeatherForecastDto { Summary = "Sunny" };
        var expectedId = 42;
        _weatherService.Setup(s => s.AddForecastAsync(dto)).ReturnsAsync(expectedId);

        // Act
        var result = await _controller.PostData(dto);

        // Assert: result.Result is CreatedAtActionResult
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult,  Is.Not.Null, "Expected a CreatedAtActionResult");

        // Assert: the returned value
        var value = createdResult.Value as WeatherForecaseReponse;
        Assert.That(value, Is.Not.Null);
        Assert.That(value.Temperature, Is.EqualTo(expectedId));
        Assert.That(value.Summary, Is.EqualTo(dto.Summary));

        // Assert: route values
        Assert.That(createdResult.RouteValues["id"], Is.EqualTo(expectedId));

    }
}