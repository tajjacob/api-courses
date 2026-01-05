using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Text.Json.Serialization;


namespace DotnetAPI.Controllers;

[ApiController] // explanation: tag the class as an API controller to enable API-specific behaviors and features 
[Route("[controller]")] // explanation: define the route template for the controller
public class UserEFController : ControllerBase // explanation: inherit from ControllerBase to gain access to common API controller functionalities
{
    DataContextEF _entityFramework;   

     public UserEFController(IConfiguration config )
    {
        _entityFramework = new DataContextEF(config);
    }


    [HttpGet("GetUsers")] 
    // public IActionResult Test()
    public IEnumerable<User> GetUsers()
    {
    IEnumerable<User> users = _entityFramework.Users.ToList<User>(); // explanation: execute the SQL query and retrieve the results as a list of User objects
        return users; // explanation: return the list of users to the client
    }   
    
    [HttpGet("GetSingleUser/{userId}")] 
    // public IActionResult Test()
    public IActionResult GetSingleUser(int userId)
    {
        
    User? user = _entityFramework.Users
    .Where(
        u => u.UserId == userId
    )
    .FirstOrDefault<User>(); // explanation: SQL query to select single user from the Users table based on userId
    if (user != null)
    {
        return Ok(user); // explanation: return the user to the client
    }
        return NotFound("Failed to get user.");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDB = _entityFramework.Users
        .Where(
            u => u.UserId == user.UserId
        )
        .FirstOrDefault<User>(); // explanation: SQL query to select single user from the Users table based on userId
     
        if (userDB != null)
        {
            userDB.Active =  user.Active;
            userDB.FirstName =  user.FirstName;
            userDB.LastName =  user.LastName;
            userDB.Email =  user.Email;
            userDB.Gender = user.Gender;
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok("User updated successfully.");
            }
            throw new Exception("Failed to update user.");
        }
        return NotFound("Failed to get user.");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDB = new User();
        
        userDB.Active =  user.Active;
        userDB.FirstName =  user.FirstName;
        userDB.LastName =  user.LastName;
        userDB.Email =  user.Email;
        userDB.Gender = user.Gender;

        _entityFramework.Users.Add(userDB);
        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok("User added successfully.");
        }
        throw new Exception("Failed to add user.");
            

    }


    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDB = _entityFramework.Users
        .Where(
            u => u.UserId == userId
        )
        .FirstOrDefault<User>(); // explanation: SQL query to select single user from the Users table based on userId
     
        if (userDB != null)
        {
            _entityFramework.Users.Remove(userDB);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok("User deleted successfully.");
            }
            throw new Exception("Failed to delete user.");
        }
        return NotFound("Failed to get user.");
    }
    
    
}
