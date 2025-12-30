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

    [HttpGet("GetUsers")] 
    // public IActionResult Test()
    public IEnumerable<User> GetUsers()
    {
     string sql = @"
     SELECT [UserId],
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active] 
    FROM TutorialAppSchema.Users"; // explanation: SQL query to select all users from the Users table
    IEnumerable<User> users = _dapper.LoadData<User>(sql); // explanation: execute the SQL query and retrieve the results as a list of User objects
        return users; // explanation: return the list of users to the client
    }   
    
    [HttpGet("GetSingleUser/{userId}")] 
    // public IActionResult Test()
    public User GetSingleUser(int userId)
    {
          string sql = @"
                SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
                FROM TutorialAppSchema.Users
                WHERE UserId = " + userId; // explanation: SQL query to select single user from the Users table based on userId
    User user = _dapper.LoadDataSingle<User>(sql); // explanation: execute the SQL query and retrieve the results as a single User object
        return user; // explanation: return the user to the client
    }  

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = $@"
        UPDATE TutorialAppSchema.Users
        SET [FirstName] = '{user.FirstName}',
            [LastName] = '{user.LastName}',
            [Email] = '{user.Email}',
            [Gender] = '{user.Gender}',
            [Active] = '{user.Active}'
        WHERE UserId = {user.UserId}";

        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok("User updated successfully.");
        }
        throw new Exception("Failed to update user.");
    }

        [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = $@"
INSERT INTO TutorialAppSchema.Users(
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active]
) VALUES(
    '{user.FirstName}',
    '{user.LastName}',
    '{user.Email}',
    '{user.Gender}',
    '{user.Active}'
    )";
        Console.WriteLine(sql);
         if (_dapper.ExecuteSql(sql))
        {
            return Ok("User added successfully.");
        }
        throw new Exception("Failed to add user.");

    }


    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = $@"
        DELETE FROM TutorialAppSchema.Users
        WHERE UserId = {userId}";

        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok("User deleted successfully.");
        }
        throw new Exception("Failed to delete user.");
    }
    
    
}
