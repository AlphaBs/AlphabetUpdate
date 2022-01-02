using System.Linq;
using AlphabetUpdateServer.Models;
using Microsoft.Extensions.Configuration;

namespace AlphabetUpdateServer.Services
{
    public class UserService : IUserService
    {
        private readonly User[]? users;
        
        public UserService(IConfiguration configuration)
        {
            users = configuration.GetSection("Users").Get<User[]>();
        }

        public User? GetUser(string username) => users?.FirstOrDefault(u => u.Username == username);

        public User? Authenticate(string username, string password)
        {
            var user = GetUser(username);
            if (user?.Password == password)
                return user;
            return null;
        }

        public bool IsValidUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                return false;
            
            var u = GetUser(user.Username);
            return u?.Password == user.Password;
        }
    }
}