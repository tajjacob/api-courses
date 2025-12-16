using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;


namespace DotnetAPI.Controllers;

[ApiController] // explanation: tag the class as an API controller to enable API-specific behaviors and features 
[Route("[controller]")] // explanation: define the route template for the controller
public class UserController : ControllerBase // explanation: inherit from ControllerBase to gain access to common API controller functionalities
{
    public UserController()
    {
    }


    [HttpGet("GetUsers/{testValue}")] 
    // public IActionResult Test()
    public string[] GetUsers(string testValue)
    {
     string[] responseArray = new string[] { "Test1", "Test2", testValue };
     return responseArray;
    }
}
