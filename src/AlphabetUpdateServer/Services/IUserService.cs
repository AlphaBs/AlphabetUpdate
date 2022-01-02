using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServer.Services
{
    public interface IUserService
    {
        User? GetUser(string username);
        User? Authenticate(string username, string password);
        bool IsValidUser(User user);
    }
}