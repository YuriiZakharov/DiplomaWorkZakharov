using DiplomaWork_WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork_WebSite.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        


        public UserController(ILogger<UserController> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;            
        }

        [HttpGet]
        public IActionResult Register()
        {     
            
            ViewBag.Title = "Register";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Register";
                return View(user);
            }

            // Validate email format
            if (!user.Email.Contains("@"))
            {
                ModelState.AddModelError("User.Email", "Email address is not valid.");
                ViewBag.Title = "Register";
                return View(user);
            }

            // Validate password length
            if (user.Password.Length < 8)
            {
                ModelState.AddModelError("User.Password", "Password must be at least 8 characters long.");
                ViewBag.Title = "Register";
                return View(user);
            }



            try
            {
                string userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                // Get country based on user's IP address
                string userCountry = await GetCountryByIpAddress(userIpAddress);

                // Check if the user's country is Russia
                if (userCountry.Equals("Russia", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Message"] = "Registration from your location is not allowed.";
                    return RedirectToAction("Register", "User");
                }

                // Hash the password before sending it to the API
                user.Password = HashPassword(user.Password);

                var response = await _httpClient.PostAsJsonAsync("https://localhost:44392/api/User/register", user);
                response.EnsureSuccessStatusCode();

                // Get the registered user ID from the response or other means
                int userId = await response.Content.ReadFromJsonAsync<int>();

                // Save the registered user ID to session state for further use
                _httpContextAccessor.HttpContext.Session.SetInt32("UserId", userId);

                TempData["Message"] = "Registration successful";
                return RedirectToAction("Welocome", "Home");
            }
            catch (Exception)
            {

                ViewBag.Title = "Register";
                return View(user);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            password = HashPassword(password);
            var loginRequest = new { Email = email, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:44392/api/User/login", content);

            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(userJson);
                _httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Handle invalid credentials error
                TempData["ErrorMessage"] = "Invalid credentials";
            }
            else
            {
                // Handle other errors
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
            }

            return View();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Отримати ідентифікатор користувача з сесії
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Отримати дані користувача за його ідентифікатором з апі
            var user = await GetUserFromApi(userId);
            user.Id = userId;

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User user, IFormFile picture)
        {
            try
            {
                // Зберегти зображення, якщо воно було змінено
                if (picture != null && picture.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        picture.CopyTo(memoryStream);
                        user.Picture = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }

                // Викликати ендпоінт API для оновлення даних користувача
                var response = await _httpClient.PutAsJsonAsync("https://localhost:44392/api/User/user/update", user);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return BadRequest("Failed to update user profile");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to update user profile");
            }
        }

        private async Task<User> GetUserFromApi(int userId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:44392/api/User/{userId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            else
            {
                // Обробити помилку отримання даних користувача з апі
                // Наприклад, повернути стандартні значення або порожнього користувача
                return new User();
            }
        }

        private async Task<string> GetCountryByIpAddress(string ipAddress)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"https://ipapi.co/{ipAddress}/country/");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception("Failed to retrieve country information from IP address.");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
