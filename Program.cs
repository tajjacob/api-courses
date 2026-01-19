// command to run the application with HTTPS profile
// dotnet run --launch-profile https
// dotnet --info
// dotnet clean
// dotnet build -v:m
// dotnet run
// to kill process on port 5050
// lsof -i :5050
// kill -9 <PID>

using System.Text.Json.Serialization;
using DotnetAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // explanation: register controller services to the dependency injection container

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi(); // explanation: use to add OpenAPI/Swagger services
builder.Services.AddEndpointsApiExplorer(); // explanation: use to explore API endpoints for Swagger
builder.Services.AddSwaggerGen(); // explanation: use to generate Swagger documentation

builder.Services.AddCors((options) => // explanation: configure CORS policies. CORS means Cross-Origin Resource Sharing. it is a security feature implemented by browsers to restrict web applications running on one origin (domain) from accessing resources on a different origin.
    {
        options.AddPolicy("DevCors", (corsBuilder) => // explanation: define a CORS policy named "DevCors" for development environment
            {
                corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000") // explanation: specify the allowed origins (domains) that can access the API during development
                    .AllowAnyMethod() // explanation: allow any HTTP method (GET, POST, PUT, DELETE, etc.) from the specified origins 
                    .AllowAnyHeader() // explanation: allow any HTTP headers from the specified origins
                    .AllowCredentials(); // explanation: allow cookies and authentication information to be sent with requests from the specified origins
            });
        options.AddPolicy("ProdCors", (corsBuilder) =>
            {
                corsBuilder.WithOrigins("https://myProductionSite.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
    });
 
builder.Services.AddScoped<IUserRepository, UserRepository>(); // explanation: register the UserRepository class as the implementation of the IUserRepository interface with a scoped lifetime

var app = builder.Build();
 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors"); // explanation: apply the "DevCors" CORS policy in the development environment
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseHttpsRedirection();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.MapControllers(); // explanation: map controller routes to the request pipeline

// if (app.Environment.IsDevelopment())
// {
//     app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
// }

// app.MapGet("/weatherforecast", () =>
// {
// })
// .WithName("GetWeatherForecast");

app.Run(); // explanation: starts the web application and listens for incoming HTTP requests
