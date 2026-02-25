using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Text.Json.Serialization;


namespace DotnetAPI.Controllers;

[ApiController] // explanation: tag the class as an API controller to enable API-specific behaviors and features 
[Route("[controller]")] // explanation: define the route template for the controller
public class UserCompleteController : ControllerBase // explanation: inherit from ControllerBase to gain access to common API controller functionalities
{
    DataContextDapper _dapper;   

     public UserCompleteController(IConfiguration config )
    {
        // Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")] // explanation: define an HTTP GET endpoint at /UserComplete/TestConnection

    public DateTime TestConnection()
    {
        var sql = "SELECT GETDATE()"; // explanation: SQL query to get the current date and time from the database server
        var currentDate = _dapper.LoadDataSingle<DateTime>(sql); // explanation: execute the SQL query and retrieve the result as a DateTime object
        return currentDate; // explanation: return the current date and time to the client
    }

    [HttpGet("GetUsers/{userId}/{isActive}")] 
    // public IActionResult Test()
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
     string sql = @"
     EXEC TutorialAppSchema.spUsers_Get";
    if (userId != 0)
        {
            sql += " @UserId = " + userId; // explanation: SQL query to execute the stored procedure that retrieves complete user information, optionally filtering by userId if it's not zero  
        }
    if (isActive)
        {
            sql += " @Active = " + isActive; // explanation: SQL query to execute the stored procedure that retrieves complete user information, optionally filtering by isActive if it's not false
        }
    IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql); // explanation: execute the SQL query and retrieve the results as a list of UserComplete objects
        return users; // explanation: return the list of users to the client
    }   
    

    [HttpPut("UpsertUser")]
    public IActionResult EditUser(UserComplete user)
    {
        string sql = $@"
        EXEC TutorialAppSchema.spUsers_Upsert
        @FirstName = '" + user.FirstName + @"',
        @LastName = '" + user.LastName + @"',
        @Email = '" + user.Email + @"',
        @Gender = '" + user.Gender + @"',
        @Active = '" + user.Active + @"',
        @UserId = '" + user.UserId + @"'";




        Console.WriteLine(sql);
        if (_dapper.ExecuteSql(sql))
        {
            return Ok("User updated successfully.");
        }
        throw new Exception("Failed to update user.");
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
    
    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = " + userId.ToString();
        
        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        } 

        throw new Exception("Failed to Delete User");
    }
    
    
}
