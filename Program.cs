using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // explanation: register controller services to the dependency injection container

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
    app.UseHttpsRedirection();
}

app.MapControllers(); // explanation: map controller routes to the request pipeline

// app.MapGet("/weatherforecast", () =>
// {
// })
// .WithName("GetWeatherForecast");

app.Run(); // explanation: starts the web application and listens for incoming HTTP requests


