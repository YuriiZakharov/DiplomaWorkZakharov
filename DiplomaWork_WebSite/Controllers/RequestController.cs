using DiplomaWork_WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace DiplomaWork_WebSite.Controllers
{
    public class RequestController : Controller
    {
        private readonly ILogger<RequestController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestController(ILogger<RequestController> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public class PlaceRequestsViewModel
        {
            public List<Request> Requests { get; set; }
            public Dictionary<int, (string Name, string LastName)> UserDetails { get; set; }
        }

        public class UserRequestsViewModel
        {
            public List<Request> Requests { get; set; }
            public List<Place> Places { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaceRequests(int placeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:44392/api/Request/place/requests?placeId={placeId}");

                if (response.IsSuccessStatusCode)
                {
                    var requestsJson = await response.Content.ReadAsStringAsync();
                    var requests = JsonConvert.DeserializeObject<List<Request>>(requestsJson);

                    // Dictionary to store user details by user ID
                    var userDetails = new Dictionary<int, (string UserName, string LastName)>();

                    foreach (var request in requests)
                    {
                        // Get user details for each request
                        var userResponse = await _httpClient.GetAsync($"https://localhost:44392/api/User/{request.UserId}");
                        if (userResponse.IsSuccessStatusCode)
                        {
                            var userJson = await userResponse.Content.ReadAsStringAsync();
                            var user = JsonConvert.DeserializeObject<User>(userJson);
                            userDetails[request.UserId] = (user.Name, user.LastName);
                        }
                    }

                    var viewModel = new PlaceRequestsViewModel
                    {
                        Requests = requests,
                        UserDetails = userDetails
                    };

                    return View(viewModel);
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"HTTP request failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRequests()
        {
            try
            {
                int userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0;

                // Отримання запитів користувача
                var requestsResponse = await _httpClient.GetAsync($"https://localhost:44392/api/Request/user/requests?userId={userId}");

                if (!requestsResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)requestsResponse.StatusCode);
                }

                var requestsJson = await requestsResponse.Content.ReadAsStringAsync();
                var requests = JsonConvert.DeserializeObject<List<Request>>(requestsJson);

                // Створення списку для зберігання даних про місця
                var places = new List<Place>();

                // Додавання даних про місце до кожного запиту
                foreach (var request in requests)
                {
                    // Отримання ідентифікатора місця з кожного запиту
                    var placeId = request.PlaceId;

                    // Отримання даних про місце, використовуючи placeId з кожного запиту
                    var placeResponse = await _httpClient.GetAsync($"https://localhost:44392/api/Place/{placeId}");

                    if (placeResponse.IsSuccessStatusCode)
                    {
                        var placeJson = await placeResponse.Content.ReadAsStringAsync();
                        var place = JsonConvert.DeserializeObject<Place>(placeJson);
                        places.Add(place);
                    }
                }

                // Створення моделі для передачі на представлення
                var userRequestsViewModel = new UserRequestsViewModel
                {
                    Requests = requests,
                    Places = places
                };

                return View(userRequestsViewModel);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"HTTP request failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet]
        public IActionResult CreateRequest(int selectedPlaceId)
        {
            try
            {
                
                int userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0; 

                // Передача значень за замовчуванням
                var request = new Request
                {
                    Text = "",
                    UserId = userId,
                    PlaceId = selectedPlaceId,
                    IsAccepted = false,
                    IsDeclined = false,
                    DeclineText = "",
                    CreatedDate = DateTime.Now
                };               

                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(Request request)
        {            
            try
            {
                request.DeclineText = " ";
                request.CreatedDate = DateTime.Now;
                // Отправляем данные в API endpoint
                var response = await _httpClient.PostAsJsonAsync("https://localhost:44392/api/Request/add", request);
                response.EnsureSuccessStatusCode(); // Проверяем, что запрос успешно обработан

                // Вернемся на главную страницу
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex)
            {
                // Обработка ошибок при обращении к API
                ModelState.AddModelError(string.Empty, "Server call error: " + ex.Message);
                return View(request);
            }
        }

        [HttpPost("update/request")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            try
            {
                // Викликаємо ендпоінт API для прийняття запиту
                var response = await _httpClient.PutAsync($"https://localhost:44392/api/Request/update/request?requestId={requestId}", null);

                // Перевіряємо, чи відповідь успішна
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home"); // Повертаємо успішний статус код
                }
                else
                {
                    return StatusCode((int)response.StatusCode); // Повертаємо статус код з API
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"HTTP request failed: {ex.Message}"); // Обробляємо помилку HTTP запиту
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // Обробляємо загальну помилку
            }
        }

        [HttpDelete("delete/request")]
        public async Task<IActionResult> DeclineRequest(int requestId, string text)
        {
            try
            {
                // Викликаємо ендпоінт API для відхилення запиту
                var response = await _httpClient.DeleteAsync($"https://localhost:44392/api/Request/{requestId}?text={text}");

                // Перевіряємо, чи відповідь успішна
                if (response.IsSuccessStatusCode)
                {
                    return Ok(); // Повертаємо успішний статус код
                }
                else
                {
                    return StatusCode((int)response.StatusCode); // Повертаємо статус код з API
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"HTTP request failed: {ex.Message}"); // Обробляємо помилку HTTP запиту
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // Обробляємо загальну помилку
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteResponse(int requestId)
        {
            try
            {
                // Викликаємо ендпоінт API для відхилення запиту
                var response = await _httpClient.DeleteAsync($"https://localhost:44392/api/Request/deleteNow/{requestId}");

                // Перевіряємо, чи відповідь успішна
                if (response.IsSuccessStatusCode)
                {
                    return Ok(); 
                }
                else
                {
                    return StatusCode((int)response.StatusCode); 
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"HTTP request failed: {ex.Message}"); // Обробляємо помилку HTTP запиту
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // Обробляємо загальну помилку
            }
        }
    }
}
