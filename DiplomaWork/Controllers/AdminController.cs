using DiplomaWork.Models;
using DiplomaWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DiplomaWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [HttpPost("login")]
        public ActionResult<Admin> Login([FromBody] AdminLoginRequest loginRequest)
        {
            var admin = _adminRepository.Login(loginRequest.Email, loginRequest.Password, loginRequest.AdminCode);
            if (admin != null)
            {
                return Ok(admin);
            }
            return NotFound("Invalid credentials");
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] Admin admin)
        {
            if (_adminRepository.Register(admin))
            {
                return Ok("Registration successful");
            }
            return BadRequest("Invalid admin data");
        }

        [HttpPut("admin/update")]
        public ActionResult UpdateAdmin([FromBody] Admin admin)
        {
            try
            {
                var existingAdmin = _adminRepository.GetAdminById(admin.Id);

                if (existingAdmin == null)
                {
                    return NotFound("Admin not found");
                }

                existingAdmin.Name = admin.Name;
                existingAdmin.LastName = admin.LastName;
                existingAdmin.Email = admin.Email;
                existingAdmin.PhoneNum = admin.PhoneNum;
                existingAdmin.Password = admin.Password;
                existingAdmin.PersonalCode = admin.PersonalCode;

                _adminRepository.UpdateAdmin(existingAdmin);

                return Ok("Admin updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to update admin");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAdmin(int id)
        {
            var admin = _adminRepository.GetAdmins().FirstOrDefault(u => u.Id == id);
            if (admin == null)
            {
                return NotFound("Admin not found");
            }

            if (_adminRepository.DeleteAdmin(admin))
            {
                return Ok("Admin deleted successfully");
            }

            return BadRequest("Failed to delete admin");
        }
    }

    public class AdminLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string AdminCode { get; set; }
    }
}