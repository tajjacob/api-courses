using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Text.Json.Serialization;


namespace DotnetAPI.Controllers;

[ApiController] // explanation: tag the class as an API controller to enable API-specific behaviors and features 
[Route("[controller]")] // explanation: define the route template for the controller
public class UserController : ControllerBase // explanation: inherit from ControllerBase to gain access to common API controller functionalities
{
    DataContextDapper _dapper;   

     public UserController(IConfiguration config )
    {
        // Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")] // explanation: define an HTTP GET endpoint at /User/TestConnection

    public DateTime TestConnection()
    {
        var sql = "SELECT GETDATE()"; // explanation: SQL query to get the current date and time from the database server
        var currentDate = _dapper.LoadDataSingle<DateTime>(sql); // explanation: execute the SQL query and retrieve the result as a DateTime object
        return currentDate; // explanation: return the current date and time to the client
    }

    [HttpGet("GetUsers/{testValue}")] 
    // public IActionResult Test()
    public string[] GetUsers(string testValue)
    {
     string[] responseArray = new string[] { "Test1", "Test2", testValue };
     return responseArray;
    }
}
