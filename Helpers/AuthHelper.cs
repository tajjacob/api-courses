using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
using Dapper;
namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
      private readonly DataContextDapper _dapper;

    public AuthHelper(IConfiguration config)
    {
      _dapper =  new DataContextDapper(config);
          _config = config;
    }

     public byte[] GetPasswordHash(string password, byte[] passwordSalt)
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

    public string CreateToken(int userId)
    {
      Claim[] claims = new Claim[]
      {
        new Claim("userId", userId.ToString())
      };

      string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

      SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(
          tokenKeyString != null ? tokenKeyString :""
          )
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

    public bool SetPassword(UserForLoginDto userForLogin)
    {
      byte[] passwordSalt = new byte[128 / 8];
          using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
          {
            rng.GetNonZeroBytes(passwordSalt);
          }
          
          byte[] passwordHash = GetPasswordHash(userForLogin.Password, passwordSalt);

          string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
          @Email = @EmailParam, 
          @PasswordHash = @PasswordHashParam, 
          @PasswordSalt = @PasswordSaltParam";
          // left is sp parameter, right is C# parameter

          List<SqlParameter> sqlParameters = new List<SqlParameter>();

          SqlParameter emailParameter = new SqlParameter(
            "@EmailParam", SqlDbType.NVarChar);
          emailParameter.Value = userForLogin.Email;  
          sqlParameters.Add(emailParameter);
          

          SqlParameter passwordHashParameter = new SqlParameter(
            "@PasswordHashParam", SqlDbType.VarBinary);
          passwordHashParameter.Value = passwordHash;  
          sqlParameters.Add(passwordHashParameter);
          
          
          SqlParameter passwordSaltParameter = new SqlParameter(
            "@PasswordSaltParam", SqlDbType.VarBinary);
          passwordSaltParameter.Value = passwordSalt;  
          sqlParameters.Add(passwordSaltParameter);
        
          
          return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
    
    
      
    }


    }
}