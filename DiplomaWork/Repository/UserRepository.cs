using DiplomaWork.Data;
using DiplomaWork.Interfaces;
using DiplomaWork.Models;
using System.Security.Cryptography;
using System.Text;

namespace DiplomaWork.Repisitory
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public bool DeleteUser(User user)
        {
            _context.Remove(user);
            return Save();
        }

        public bool DeleteUserWithData(int id)
        {
           var user = _context.Users.FirstOrDefault(x => x.Id == id);
           foreach(var place in _context.Places.ToList())
               if (place.HostId == id)
                    _context.Remove(place);
           foreach(var chat in _context.Chats.Where(x => x.HostId == id || x.UserId == id))
                _context.Remove(chat);
            _context.Remove(user);
            return Save();
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public int GetUserIdByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            return user.Id;
        }

        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy(x => x.Id).ToList();
        }
        public User Login(string data, string password)
        {         
            User user = _context.Users.FirstOrDefault(u => u.Email == data || u.PhoneNum == data);                
            if (user != null && user.Password == password)
            {                    
                return user;
            }                
            return null;            
        }

        public bool Register(User user)
        {
            if(user.PhoneNum.Length <= 15 && user.Email.Contains('@') && user.Password.Length >= 8)
            {
                _context.Users.Add(user);
                
            }
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateUser(User user)
        {
            _context.Update(user);
            return Save();
        }

        public bool UserExists(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id) != null;
        }
    }
}
