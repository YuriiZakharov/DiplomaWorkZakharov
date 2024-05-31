using DiplomaWork_WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DiplomaWork_WebSite.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(ILogger<AdminController> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginRequest loginRequest)
        {
            try
            {
                var apiClient = _httpClientFactory.CreateClient();
                apiClient.BaseAddress = new Uri("https://localhost:44392/api/Admin/login"); // Замініть це на адресу вашого API

                var response = await apiClient.PostAsJsonAsync("login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var admin = await response.Content.ReadFromJsonAsync<Admin>();
                    // Ваш код після успішної аутентифікації, наприклад, збереження токена або сесії
                    return RedirectToAction("UserList", "Admin"); // Перенаправлення на головну сторінку після успішної аутентифікації
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return View(loginRequest); // Повертаємо знову форму логіну з помилками
        }

        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            try
            {                
                var response = await _httpClient.GetAsync("https://localhost:44392/api/User");

                if (response.IsSuccessStatusCode)
                {
                    var usersJson = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<User>>(usersJson);
                    return View(users);
                }
                else
                {
                    // Обробка помилки
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(new List<User>()); // Повертаємо порожній список у разі помилки
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"https://localhost:44392/api/User/{id}/data");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("UserList"); // Перенаправлення на список користувачів після успішного видалення
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to delete user. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return RedirectToAction("UserList"); // Перенаправлення на список користувачів у разі помилки
        }

        [HttpGet]
        public async Task<IActionResult> UserMessages(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:44392/api/Message/user/{userId}");

                if (response.IsSuccessStatusCode)
                {                    
                    var messagesJson = await response.Content.ReadAsStringAsync();
                    var messages = JsonConvert.DeserializeObject<List<Message>>(messagesJson);
                    return View(messages);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve messages. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return View(new List<Message>()); // Перенаправлення на представлення з порожнім списком у разі помилки
        }

        [HttpGet]
        public async Task<IActionResult> UserPlaces(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:44392/api/Place/places/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var placesJson = await response.Content.ReadAsStringAsync();
                    var places = JsonConvert.DeserializeObject<List<Place>>(placesJson);
                    return View(places);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve places. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return View(new List<Place>()); // Перенаправлення на представлення з порожнім списком у разі помилки
        }

        [HttpGet]
        public async Task<IActionResult> PlaceImages(int placeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:44392/api/Image/{placeId}");

                if (response.IsSuccessStatusCode)
                {
                    var imagesJson = await response.Content.ReadAsStringAsync();
                    var images = JsonConvert.DeserializeObject<List<Image>>(imagesJson);
                    return View(images);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve images. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            return View(new List<Image>()); // Перенаправлення на представлення з порожнім списком у разі помилки
        }

        public class AdminLoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string AdminCode { get; set; }
        }
    }
}
