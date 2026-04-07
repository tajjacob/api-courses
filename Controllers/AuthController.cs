using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
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
          byte[] passwordSalt = new byte[128 / 8];
          using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
          {
            rng.GetNonZeroBytes(passwordSalt);
          }
          
          byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

          string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
          @Email = @EmailParam, 
          @PasswordHash = @PasswordHashParam, 
          @PasswordSalt = @PasswordSaltParam";
          // left is sp parameter, right is C# parameter

          List<SqlParameter> sqlParameters = new List<SqlParameter>();

           SqlParameter emailParameter = new SqlParameter(
            "@EmailParam", SqlDbType.NVarChar);
          emailParameter.Value = userForRegistration.Email;  
          sqlParameters.Add(emailParameter);
          

          SqlParameter passwordHashParameter = new SqlParameter(
            "@PasswordHashParam", SqlDbType.VarBinary);
          passwordHashParameter.Value = passwordHash;  
          sqlParameters.Add(passwordHashParameter);
          
          
          SqlParameter passwordSaltParameter = new SqlParameter(
            "@PasswordSaltParam", SqlDbType.VarBinary);
          passwordSaltParameter.Value = passwordSalt;  
          sqlParameters.Add(passwordSaltParameter);
        
          
          if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
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
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {

      string sqlForHashAndSalt = @"SELECT [Email],
                                [PasswordHash],
                                [PasswordSalt]
                                FROM TutorialAppSchema.Auth WHERE Email = '" + userForLogin.Email + "'";
      
      var userForLoginConfirmationList = _dapper.LoadData<UserForLoginConfirmationDto>(sqlForHashAndSalt);
      if (userForLoginConfirmationList.Count() == 0)
      {
        return StatusCode(401, "Invalid email");
      }
      UserForLoginConfirmationDto userForLoginConfirmation = userForLoginConfirmationList.First();

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