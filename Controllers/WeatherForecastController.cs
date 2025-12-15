using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;


namespace DotnetAPI.Controllers;

[ApiController] // explanation: tag the class as an API controller to enable API-specific behaviors and features 
[Route("[controller]")] // explanation: define the route template for the controller
public class WeatherForecastController : ControllerBase // explanation: inherit from ControllerBase to gain access to common API controller functionalities
{
    private readonly string[] _summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    [HttpGet("", Name = "GetWeatherForecast")] // explanation: specify that this action responds to HTTP GET requests and name the route
    public IEnumerable<WeatherForecast> GetFiveDayForecast()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
      new WeatherForecast
      (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          _summaries[Random.Shared.Next(_summaries.Length)]
      ))
      .ToArray();
        return forecast;
    }
}

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) // explanation: defines a record type to represent weather forecast data
public record WeatherForecast(DateOnly Date, [property: JsonPropertyName("tempC")] int TemperatureC, string? Summary) // explanation: defines a record type to represent weather forecast data

{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
