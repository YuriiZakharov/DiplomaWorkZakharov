using DiplomaWork.Models;

namespace DiplomaWork.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        User Login(string data, string password);
        User GetUserById(int id);
        int GetUserIdByEmail(string email);
        bool UserExists(int id);
        bool Register(User user);
        bool UpdateUser(User user);
        bool DeleteUser(User user);
        bool DeleteUserWithData(int id);
        bool Save();
    }
}
