using System.Data;
using System.Data.Common;
using System.Text.Json.Serialization;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;


namespace DotnetAPI.Controllers;

[ApiController] // explanation: tag the class as an API controller to enable API-specific behaviors and features 
[Route("[controller]")] // explanation: define the route template for the controller
public class UserCompleteController : ControllerBase // explanation: inherit from ControllerBase to gain access to common API controller functionalities
{
    private readonly DataContextDapper _dapper;

    private readonly ReusableSql _reusableSql;

    public UserCompleteController(IConfiguration config)
    //instructor for the UserCompleteController class that takes an IConfiguration object as a parameter, 
    // which is used to initialize the DataContextDapper and ReusableSql instances for database operations
    {
        {
            _dapper = new DataContextDapper(config);
            _reusableSql = new ReusableSql(config);
        }
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

        string stringParameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();

        if (userId != 0)
        {
            stringParameters += ", @UserId=@UserIdParameter"; // explanation: add the userId parameter to the SQL query if it's not 0, using a parameterized query to prevent SQL injection
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32); // explanation: add the userId parameter to the DynamicParameters object to be used in the SQL query execution
        }
        if (isActive)
        {
            stringParameters += ", @Active=@ActiveParameter"; // explanation: add the isActive parameter to the SQL query if it's true, using a parameterized query to prevent SQL injection
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean); // explanation: add the isActive parameter to the DynamicParameters object to be used in the SQL query execution
        }
        if (stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1); // explanation: append the string parameters to the SQL query, removing the leading comma if there are any string parameters

        }
        IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters); // explanation: execute the SQL query and retrieve the results as a list of UserComplete objects
        return users; // explanation: return the list of users to the client
    }


    [HttpPut("UpsertUser")]
    public IActionResult EditUser(UserComplete user)
    {
        if (_reusableSql.UpsertUser(user))
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
        WHERE UserId = @UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);


        Console.WriteLine(sql);
        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok("User deleted successfully.");
        }
        throw new Exception("Failed to delete user.");
    }

    [HttpDelete("DeleteJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = @UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

        Console.WriteLine(sql);

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }


}
