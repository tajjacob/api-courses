using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Dapper;

namespace DotnetAPI.Controllers

{
      
    [Authorize]  
    [ApiController] // explanation: tag the class as an API controller to enable API-specific behaviors and features 
    [Route("[controller]")] // explanation: define the route template for the controller
  public class AuthController : ControllerBase
  {
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _config;
    private readonly AuthHelper _authHelper;

    public AuthController(IConfiguration config)
    {
      _dapper = new DataContextDapper(config);
      _config = config;
      _authHelper = new AuthHelper(config);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    { 
      if (userForRegistration.Password == userForRegistration.PasswordConfirm)
      {
        string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" 
                                    + userForRegistration.Email + "'";
        
        IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
        if (existingUsers.Count() == 0)
        {
        
        UserForLoginDto userForSetPassword = new UserForLoginDto()
        {
          Email = userForRegistration.Email,
          Password = userForRegistration.Password,
        };
          
          if (_authHelper.SetPassword(userForSetPassword))
          {

                   string sqlAddUser = $@"
                    EXEC TutorialAppSchema.spUsers_Upsert
                    @FirstName = '{userForRegistration.FirstName}',
                    @LastName = '{userForRegistration.LastName}',
                    @Email = '{userForRegistration.Email}',
                    @Gender = '{userForRegistration.Gender}',
                    @Active = 1,
                    @JobTitle = '{userForRegistration.JobTitle}',
                    @Department = '{userForRegistration.Department}',
                    @Salary = '{userForRegistration.Salary}'";  


            // string sqlAddUser = $@"
            //             INSERT INTO TutorialAppSchema.Users(
            //                 [FirstName],
            //                 [LastName],
            //                 [Email],
            //                 [Gender],
            //                 [Active]
            //             ) VALUES(
            //                 '{userForRegistration.FirstName}',
            //                 '{userForRegistration.LastName}',
            //                 '{userForRegistration.Email}',
            //                 '{userForRegistration.Gender}',
            //                 '1'
            //                 )";

            if (_dapper.ExecuteSql(sqlAddUser))
            {
              return Ok();
            }
            throw new Exception("Failed to add user");

          }
          throw new Exception("Failed to register user");
        }
        throw new Exception("User already exists");
      }
      throw new Exception("Passwords do not match");



      
    }

    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
      if (_authHelper.SetPassword(userForSetPassword))
      {
        return Ok();
      }
      throw new Exception("Failed to update password");
    }

    
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {

      string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get
       @Email = @EmailParam";
       // left is sp parameter, right is C# parameter

       DynamicParameters sqlParameters = new DynamicParameters();
       sqlParameters.Add("@EmailParam", userForLogin.Email);
      

      UserForLoginConfirmationDto? userForLoginConfirmation = 
      _dapper.LoadDataWithParameters<UserForLoginConfirmationDto>(
        sqlForHashAndSalt, sqlParameters
      ).FirstOrDefault();


      if (userForLoginConfirmation == null)
      {
        return StatusCode(401, "Invalid email or password");
      }

      byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

      for (int i = 0; i < passwordHash.Length; i++)
      {
        if (passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
        {
          return StatusCode(401, "Invalid password");
        }
      }


      string sqlForUserId = @"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" 
                        + userForLogin.Email + "'";

      int userId = _dapper.LoadDataSingle<int>(
        sqlForUserId
      );

      return Ok(
        new Dictionary<string, string>
        {
          { "token", _authHelper.CreateToken(userId) }
        }
        );
      
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
      string userId = User.FindFirst("userId")?.Value ?? "";

      string sqlForUserId = @"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = " 
                        + userId;

      int userIdFromDB = _dapper.LoadDataSingle<int>(
        sqlForUserId
      );


      return Ok(
        new Dictionary<string, string>
        {
          { "token", _authHelper.CreateToken(userIdFromDB) }
        }
        );
    }

   

  }


}