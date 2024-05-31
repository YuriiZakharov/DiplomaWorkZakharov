using DiplomaWork.Data;
using DiplomaWork.Interfaces;
using DiplomaWork.Models;
using System.Security.Cryptography;
using System.Text;

namespace DiplomaWork.Repisitory
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;

        public AdminRepository(DataContext context)
        {
            _context = context;
        }

        public bool AdminExists(int id)
        {
           return _context.Admins.FirstOrDefault(x => x.Id == id) != null;
        }

        public bool DeleteAdmin(Admin admin)
        {
            _context.Remove(admin);
            return Save();
        }

        public Admin GetAdminById(int id)
        {
           return _context.Admins.FirstOrDefault(y => y.Id == id);
        }

        public ICollection<Admin> GetAdmins()
        {
            return _context.Admins.OrderBy(x => x.Id).ToList();
        }

        public Admin Login(string data, string password, string adminCode)
        {
            Admin admin = _context.Admins.FirstOrDefault(u => u.Email == data || u.PhoneNum == data);

            if (admin != null && admin.Password == password && admin.PersonalCode == adminCode)
            {
                return admin;
            }
            return null;            
        }

        

        public bool Register(Admin admin)
        {
            if (admin.PhoneNum.Length <= 15 && admin.Email.Contains('@') && admin.Password.Length >= 8)            
                _context.Admins.Add(admin);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateAdmin(Admin admin)
        {
            _context.Update(admin);
            return Save();
        }
    }
}
