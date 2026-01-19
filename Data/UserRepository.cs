using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
         DataContextEF _entityFramework;   
        public UserRepository(IConfiguration config )
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return (_entityFramework.SaveChanges() > 0);
        }

        public void AddEntity<T>(T entityToAdd)
        {   
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        } 

        public void RemoveEntity<T>(T entityToRemove)
        {   
            if (entityToRemove != null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }

         public IEnumerable<User> GetUsers()
        {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>(); // explanation: execute the SQL query and retrieve the results as a list of User objects
            return users; // explanation: return the list of users to the client
        }

        public User GetSingleUser(int userId)
        {

            User? user = _entityFramework.Users
            .Where(
                u => u.UserId == userId
            )
            .FirstOrDefault<User>(); // explanation: SQL query to select single user from the Users table based on userId
            if (user != null)
            {
                return user; // explanation: return the user to the client
            }
            return null;
        }

        public UserSalary GetSingleUserSalary(int userId)
        {

            UserSalary? userSalary = _entityFramework.UserSalary
            .Where(
                u => u.UserId == userId
            )
            .FirstOrDefault<UserSalary>(); // explanation: SQL query to select single user salary from the UserSalary table based on userId
            if (userSalary != null)
            {
                return userSalary; // explanation: return the user salary to the client
            }
            throw new Exception("Failed to get user salary.");
        } 

        public UserJobInfo GetSingleUserJobInfo(int userId)
        {

            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
            .Where(
                u => u.UserId == userId
            )
            .FirstOrDefault<UserJobInfo>(); // explanation: SQL query to select single user job info from the UserJobInfo table based on userId
            if (userJobInfo != null)
            {
                return userJobInfo; // explanation: return the user job info to the client
            }
            throw new Exception("Failed to get user job info.");
        }
    }
}