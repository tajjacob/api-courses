using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace DotnetAPI.Controllers

{
  public class AuthController : ControllerBase
  {
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
      _dapper = new DataContextDapper(config);
      _config = config;
    }

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
          
          byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

          string sqlAddAuth = @"
          INSERT INTO TutorialAppSchema.Auth(
          [Email],
          [PasswordHash],
          [PasswordSalt]
          ) VALUES( '" + userForRegistration.Email + "', @PasswordHash, @PasswordSalt )";

          List<SqlParameter> sqlParameters = new List<SqlParameter>();

          SqlParameter passwordSaltParameter = new SqlParameter(
            "@PasswordSalt", SqlDbType.VarBinary);
          passwordSaltParameter.Value = passwordSalt;

          SqlParameter passwordHashParameter = new SqlParameter(
            "@PasswordHash", SqlDbType.VarBinary);
          passwordHashParameter.Value = passwordHash;

          sqlParameters.Add(passwordSaltParameter);
          sqlParameters.Add(passwordHashParameter);
          
          if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
          {
            string sqlAddUser = $@"
                        INSERT INTO TutorialAppSchema.Users(
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]
                        ) VALUES(
                            '{userForRegistration.FirstName}',
                            '{userForRegistration.LastName}',
                            '{userForRegistration.Email}',
                            '{userForRegistration.Gender}',
                            '1'
                            )";

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

      byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

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
          { "token", CreateToken(userId) }
        }
        );
      
    }

    private byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
      string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value 
                                      + Convert.ToBase64String(passwordSalt);
      
      byte[] passwordHash = KeyDerivation.Pbkdf2(
        password: password,
        salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 10000,
        numBytesRequested: 256 / 8
      );
      return passwordHash;
    }

    private string CreateToken(int userId)
    {
      Claim[] claims = new Claim[]
      {
        new Claim("userId", userId.ToString())
      };

      SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:TokenKey").Value)
      );
      SigningCredentials credentials = new SigningCredentials(
        tokenKey, SecurityAlgorithms.HmacSha512Signature
      );
      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = credentials
      };
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);

    
    }

  }
}