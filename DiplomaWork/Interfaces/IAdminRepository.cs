using DiplomaWork.Models;

namespace DiplomaWork.Interfaces
{
    public interface IAdminRepository
    {
        ICollection<Admin> GetAdmins();
        Admin Login(string data, string password, string adminCode);
        Admin GetAdminById(int id);
        bool Register(Admin admin);        
        bool UpdateAdmin(Admin admin);
        bool DeleteAdmin(Admin admin);
        bool AdminExists(int id);
        bool Save();
        
    }
}
