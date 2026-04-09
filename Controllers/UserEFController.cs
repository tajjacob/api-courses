// using AutoMapper;
// using DotnetAPI.Data;
// using DotnetAPI.Dtos;
// using DotnetAPI.Models;
// using Microsoft.AspNetCore.Mvc;


// namespace DotnetAPI.Controllers;

// [ApiController] // explanation: tag the class as an API controller to enable API-specific behaviors and features 
// [Route("[controller]")] // explanation: define the route template for the controller
// public class UserEFController : ControllerBase // explanation: inherit from ControllerBase to gain access to common API controller functionalities
// {
//     // DataContextEF _entityFramework;  

//     IUserRepository _userRepository;
//     IMapper _mapper;

//      public UserEFController(IConfiguration config, IUserRepository userRepository)
//     {
//         // _entityFramework = new DataContextEF(config);
//         _userRepository = userRepository;

//         _mapper = new MapperConfiguration(cfg =>
//         {
//             cfg.CreateMap<UserToAddDto, User>();
//         }).CreateMapper();
//     }


//     [HttpGet("GetUsers")] 
//     // public IActionResult Test()
//     public IEnumerable<User> GetUsers()
//     {
//     IEnumerable<User> users = _userRepository.GetUsers(); // explanation: execute the SQL query and retrieve the results as a list of User objects
//         return users; // explanation: return the list of users to the client
//     }   
    
//     [HttpGet("GetSingleUser/{userId}")] 
//     // public IActionResult Test()
//     public User GetSingleUser(int userId)
//     {

//         return _userRepository.GetSingleUser(userId);
        
//     // User? user = _entityFramework.Users
//     // .Where(
//     //     u => u.UserId == userId
//     // )
//     // .FirstOrDefault<User>(); // explanation: SQL query to select single user from the Users table based on userId
//     // if (user != null)
//     // {
//     //     return Ok(user); // explanation: return the user to the client
//     // }
//     //     return NotFound("Failed to get user.");
//     }

//     [HttpPut("EditUser")]
//     public IActionResult EditUser(User user)
//     {
//         User? userDB =_userRepository.GetSingleUser(user.UserId);
//         // User? userDB = _entityFramework.Users
//         // .Where(
//         //     u => u.UserId == user.UserId
//         // )
//         // .FirstOrDefault<User>(); // explanation: SQL query to select single user from the Users table based on userId
     
//         if (userDB != null)
//         {
//             userDB.Active =  user.Active;
//             userDB.FirstName =  user.FirstName;
//             userDB.LastName =  user.LastName;
//             userDB.Email =  user.Email;
//             userDB.Gender = user.Gender;
//             if (_userRepository.SaveChanges())
//             {
//                 return Ok("User updated successfully.");
//             }
//             throw new Exception("Failed to update user.");
//         }
//         return NotFound("Failed to get user.");
//     }

//     [HttpPost("AddUser")]
//     public IActionResult AddUser(UserToAddDto user)
//     {
//         // User userDB = new User();
//         User userDB = _mapper.Map<User>(user);
        
//         // userDB.Active =  user.Active;
//         // userDB.FirstName =  user.FirstName;
//         // userDB.LastName =  user.LastName;
//         // userDB.Email =  user.Email;
//         // userDB.Gender = user.Gender;

//         _userRepository.AddEntity<User>(userDB);
//         if (_userRepository.SaveChanges())
//         {
//             return Ok("User added successfully.");
//         }
//         throw new Exception("Failed to add user.");
            

//     }


//     [HttpDelete("DeleteUser/{userId}")]
//     public IActionResult DeleteUser(int userId)
//     {
//         User? userDB = _userRepository.GetSingleUser(userId);
     
//         if (userDB != null)
//         {
//             _userRepository.RemoveEntity<User>(userDB);
//             if (_userRepository.SaveChanges())
//             {
//                 return Ok("User deleted successfully.");
//             }
//             throw new Exception("Failed to delete user.");
//         }
//         return NotFound("Failed to get user.");
//     }

//        [HttpGet("UserSalary/{userId}")]
//     public UserSalary GetUserSalaryEF(int userId)
//     {
//         return _userRepository.GetSingleUserSalary(userId);
//     }

//       [HttpPost("UserSalary")]
//     public IActionResult PostUserSalaryEf(UserSalary userForInsert)
//     {
//         _userRepository.AddEntity<UserSalary>(userForInsert);
//         if (_userRepository.SaveChanges())
//         {
//             return Ok();
//         }
//         throw new Exception("Adding UserSalary failed on save");
//     }
    
//      [HttpPut("UserSalary")]
//     public IActionResult PutUserSalaryEf(UserSalary userForUpdate)
//     {
//         UserSalary? userToUpdate = _userRepository.GetSingleUserSalary(userForUpdate.UserId);

//         if (userToUpdate != null)
//         {
//             _mapper.Map(userForUpdate, userToUpdate);
//             if (_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Updating UserSalary failed on save");
//         }
//         throw new Exception("Failed to find UserSalary to Update");
//     }
    
//      [HttpDelete("UserSalary/{userId}")]
//     public IActionResult DeleteUserSalaryEf(int userId)
//     {
//         UserSalary? userToDelete = _userRepository.GetSingleUserSalary(userId);

//         if (userToDelete != null)
//         {
//             _userRepository.RemoveEntity<UserSalary>(userToDelete);
//             if (_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Deleting UserSalary failed on save");
//         }
//         throw new Exception("Failed to find UserSalary to delete");
//     }

//      [HttpGet("UserJobInfo/{userId}")]
//     public UserJobInfo GetUserJobInfoEF(int userId)
//     {
//         return _userRepository.GetSingleUserJobInfo(userId);
//     }

//     [HttpPost("UserJobInfo")]
//     public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
//     {
//         _userRepository.AddEntity<UserJobInfo>(userForInsert);
//         if (_userRepository.SaveChanges())
//         {
//             return Ok();
//         }
//         throw new Exception("Adding UserJobInfo failed on save");
//     }

//     [HttpPut("UserJobInfo")]
//     public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
//     {
//         UserJobInfo? userToUpdate = _userRepository.GetSingleUserJobInfo(userForUpdate.UserId);

//         if (userToUpdate != null)
//         {
//             _mapper.Map(userForUpdate, userToUpdate);
//             if (_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Updating UserJobInfo failed on save");
//         }
//         throw new Exception("Failed to find UserJobInfo to Update");
//     }

//        [HttpDelete("UserJobInfo/{userId}")]
//     public IActionResult DeleteUserJobInfoEf(int userId)
//     {
//         UserJobInfo? userToDelete = _userRepository.GetSingleUserJobInfo(userId);


//         if (userToDelete != null)
//         {
//             _userRepository.RemoveEntity<UserJobInfo>(userToDelete);
//             if (_userRepository.SaveChanges())
//             {
//                 return Ok();
//             }
//             throw new Exception("Deleting UserJobInfo failed on save");
//         }
//         throw new Exception("Failed to find UserJobInfo to delete");
//     }




// }
