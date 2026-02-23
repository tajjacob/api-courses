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

    [HttpGet("UserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalary(int userId)
    {
        return _dapper.LoadData<UserSalary>(@"
            SELECT UserSalary.UserId
                    , UserSalary.Salary
            FROM  TutorialAppSchema.UserSalary
                WHERE UserId = " + userId.ToString());
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary (
                UserId,
                Salary
            ) VALUES (" + userSalaryForInsert.UserId.ToString()
                + ", " + userSalaryForInsert.Salary
                + ")";

        if (_dapper.ExecuteSqlWithRowCount(sql, null) > 0)
        {
            return Ok(userSalaryForInsert);
        }
        throw new Exception("Adding User Salary failed on save");
    }

    [HttpPut("UserSalary")]
    public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    {
        string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary=" 
            + userSalaryForUpdate.Salary
            + " WHERE UserId=" + userSalaryForUpdate.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userSalaryForUpdate);
        }
        throw new Exception("Updating User Salary failed on save");
    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES (" + userJobInfoForInsert.UserId
                + ", '" + userJobInfoForInsert.Department
                + "', '" + userJobInfoForInsert.JobTitle
                + "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoForInsert);
        }
        throw new Exception("Adding User Job Info failed on save");
    }

    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
    {
        string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='" 
            + userJobInfoForUpdate.Department
            + "', JobTitle='"
            + userJobInfoForUpdate.JobTitle
            + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoForUpdate);
        }
        throw new Exception("Updating User Job Info failed on save");
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
