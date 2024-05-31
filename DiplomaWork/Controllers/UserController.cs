using DiplomaWork.Models;
using DiplomaWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DiplomaWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = _userRepository.GetUsers();
            return Ok(users);
        }

        [HttpGet("GetUserIdByEmail/{email}")]
        public ActionResult<int> GetUserIdByEmail(string email)
        {
            return Ok(_userRepository.GetUserIdByEmail(email));
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            return Ok(_userRepository.GetUserById(id));
        }

        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] LoginRequest loginRequest)
        {
            var user = _userRepository.Login(loginRequest.Email, loginRequest.Password);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("Invalid credentials");
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] User user)
        {
            if (_userRepository.Register(user))
            {
                return Ok(user.Id);
            }
            return BadRequest("Invalid user data");
        }

        [HttpPut("user/update")]
        public ActionResult UpdateUser([FromBody] User user)
        {
            try
            {
                var existingUser = _userRepository.GetUserById(user.Id);

                if (existingUser == null)
                {
                   return NotFound("User not found");
                }           

                existingUser.Name = user.Name;
                existingUser.LastName = user.LastName;
                existingUser.Picture = user.Picture;
                existingUser.Email = user.Email;
                existingUser.PhoneNum = user.PhoneNum;
                existingUser.Password = user.Password;
                existingUser.IsChatable = user.IsChatable;

                _userRepository.UpdateUser(existingUser);

                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to update user");
            }
        }

        [HttpDelete("delete/{id}")]
        public ActionResult DeleteUser(int id)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (_userRepository.DeleteUser(user))
            {
                return Ok("User deleted successfully");
            }

            return BadRequest("Failed to delete user");
        }

        [HttpDelete("{id}/data")]
        public ActionResult DeleteUserWithData(int id)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (_userRepository.DeleteUserWithData(user.Id))
            {
                return Ok("User deleted successfully");
            }

            return BadRequest("Failed to delete user");
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}