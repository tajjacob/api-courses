var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi(); // explanation: use to add OpenAPI/Swagger services
builder.Services.AddEndpointsApiExplorer(); // explanation: use to explore API endpoints for Swagger
builder.Services.AddSwaggerGen(); // explanation: use to generate Swagger documentation

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi(); // explanation: use to serve OpenAPI/Swagger UI in development environment
    app.UseSwagger(); // explanation: enables middleware to serve generated Swagger as a JSON endpoint
    app.UseSwaggerUI(); // explanation: enables middleware to serve swagger-ui (HTML, JS, CSS, etc.)
}
else
{
app.UseHttpsRedirection();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run(); // explanation: starts the web application and listens for incoming HTTP requests

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) // explanation: defines a record type to represent weather forecast data
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
