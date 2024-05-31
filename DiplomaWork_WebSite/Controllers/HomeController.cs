using DiplomaWork_WebSite.Models;
using Microsoft.AspNetCore.Mvc;
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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            ViewBag.UserId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0;
            return View();
        }

        public void ChangeLanguage(string lang)
        {
            _httpContextAccessor.HttpContext.Session.SetString("Lang", lang);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Rules()
        {
            return View();
        }

        public IActionResult Guide()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public async Task<IActionResult> PlaceList()
        {
            ViewBag.UserId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0;
            var response = await _httpClient.GetAsync("https://localhost:44392/api/Place/places");

            if (response.IsSuccessStatusCode)
            {
                var placesJson = await response.Content.ReadAsStringAsync();
                var places = JsonConvert.DeserializeObject<List<Place>>(placesJson);

                return View(places);
            }
            else
            {
                // Обработка ошибки
                return StatusCode((int)response.StatusCode);
            }
        }

        public async Task<IActionResult> UserPlaceList()
        {
            // Получаем идентификатор пользователя из сессии
            int userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                // Получаем список мест пользователя по его идентификатору
                var response = await _httpClient.GetAsync($"https://localhost:44392/api/Place/places/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var placesJson = await response.Content.ReadAsStringAsync();
                    var places = JsonConvert.DeserializeObject<List<Place>>(placesJson);

                    return View(places);
                }
                else
                {
                    // Обработка ошибки
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving places for user {userId}: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Отримати адресу з параметрів запиту або з TempData
            string address = HttpContext.Request.Query["address"];
            if (string.IsNullOrEmpty(address))
            {
                address = TempData["Address"] as string;
            }

            // Перевірити, чи адреса не є порожньою
            if (!string.IsNullOrEmpty(address))
            {
                // Передати адресу до моделі
                var model = new Place { Address = address };
                return View(model);
            }
            else
            {
                // Повернути порожню модель, якщо адреса не знайдена
                return View(new Place());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Place model)
        {
            model.HostId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0;            

            try
            {
                // Отправляем данные в API endpoint
                var response = await _httpClient.PostAsJsonAsync("https://localhost:44392/api/Place", model);
                response.EnsureSuccessStatusCode(); // Проверяем, что запрос успешно обработан

                // Вернемся на главную страницу
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex)
            {
                // Обработка ошибок при обращении к API
                ModelState.AddModelError(string.Empty, "Ошибка при обращении к серверу: " + ex.Message);
                return View(model);
            }
        }




        [HttpGet]
        public async Task<IActionResult> UpdatePlace(int id)
        {
            try
            {
                // Получаем данные выбранного места по его идентификатору
                var response = await _httpClient.GetAsync($"https://localhost:44392/api/Place/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var placeJson = await response.Content.ReadAsStringAsync();
                    var place = JsonConvert.DeserializeObject<Place>(placeJson);
                    _httpContextAccessor.HttpContext.Session.SetInt32("PlaceId", id);

                    // Передаем данные выбранного места в представление
                    return View(place);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to fetch place details";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index", "Home");
            }
        }



        [HttpPost]
        public async Task<IActionResult> UpdatePlace(Place place, string startTime, string endTime)
        {
            try
            {
                // Объединяем время начала и окончания работы в одну строку формата "HH:mm-HH:mm"
                string workingHours = $"{startTime}-{endTime}";

                // Затем присваиваем это объединенное время свойству WorkingHours объекта place
                place.WorkingHours = workingHours;

                // Отправляем данные места на API endpoint для обновления
                var response = await _httpClient.PutAsJsonAsync("https://localhost:44392/api/Place/place/update", place);

                // Проверяем, что запрос успешно обработан
                if (response.IsSuccessStatusCode)
                {
                    // Читаем ответ от API
                    var message = await response.Content.ReadAsStringAsync();
                    TempData["Message"] = message;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Обработка ошибки
                    TempData["ErrorMessage"] = "Failed to update place";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePlace(int id)
        {
            try
            {
                // Отправляем запрос на удаление места на API endpoint
                var response = await _httpClient.DeleteAsync($"https://localhost:44392/api/Place/delete/{id}");

                // Проверяем, что запрос успешно обработан
                if (response.IsSuccessStatusCode)
                {
                    // Читаем ответ от API
                    var message = await response.Content.ReadAsStringAsync();
                    TempData["Message"] = message;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Обработка ошибки
                    TempData["ErrorMessage"] = "Failed to delete place";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOwnerIdAndStartChat(int hostId)
        {
            try
            {
                int userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0;

                // Check if a chat already exists between the user and the host
                var response = await _httpClient.GetAsync($"https://localhost:44392/api/Chat/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var chatsJson = await response.Content.ReadAsStringAsync();
                    var chats = JsonConvert.DeserializeObject<List<Chat>>(chatsJson);

                    var existingChat = chats.FirstOrDefault(c => c.HostId == hostId);
                    if (existingChat != null)
                    {
                        // If a chat already exists, set the selected chat ID and redirect to the chat page
                        _httpContextAccessor.HttpContext.Session.SetInt32("SelectedChatId", existingChat.Id);
                        return RedirectToAction("StartChat", "Chat");
                    }
                }

                // Save hostId to session
                _httpContextAccessor.HttpContext.Session.SetInt32("HostId", hostId);


                var chat = new Chat
                {
                    UserId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0,
                    HostId = hostId
                };

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(chat);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response2 = await _httpClient.PostAsync("https://localhost:44392/api/Chat", content);

                if (response2.IsSuccessStatusCode)
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("SelectedChatId", chat.Id);
                    return RedirectToAction("StartChat", "Chat");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to start chat";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult SetPlaceIdInSession(int placeId)
        {
            HttpContext.Session.SetInt32("PlaceId", placeId);
            return Ok();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}