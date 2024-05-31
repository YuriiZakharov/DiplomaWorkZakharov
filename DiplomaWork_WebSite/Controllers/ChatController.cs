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
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatController(ILogger<ChatController> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> StartChat()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewData["UserId"] = userId;
            ViewData["ChatId"] = HttpContext.Session.GetInt32("SelectedChatId");
            var chatId = Convert.ToInt32(ViewData["ChatId"]);
            if (chatId != 0)
            {
                var messages = await GetChatMessages(chatId);
                return View(messages);
            }
            else
            {
                return View();
            }
        }

        public async Task<IEnumerable<Message>> GetChatMessages(int chatId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:44392/api/Message/chat/{chatId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var messages = JsonConvert.DeserializeObject<IEnumerable<Message>>(content);
                return messages;
            }
            else
            {
                _logger.LogError("Failed to retrieve chat messages. Status code: {StatusCode}", response.StatusCode);
                return null;
            }
        }

        public IActionResult ViewChat(int chatId)
        {
            _httpContextAccessor.HttpContext.Session.SetInt32("SelectedChatId", chatId);
            return RedirectToAction("StartChat", "Chat"); // Припустимо, що у вас є дія Index у контролері Chat, де ви будете відображати обраний чат
        }

        public async Task<IActionResult> ChatList()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0;
                var response = await _httpClient.GetAsync($"https://localhost:44392/api/Chat/user/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var chats = JsonConvert.DeserializeObject<IEnumerable<Chat>>(content);

                    var chatViewModels = new List<ChatViewModel>();
                    foreach (var chat in chats)
                    {
                        var chatViewModel = new ChatViewModel();
                        var userResponse = new HttpResponseMessage();
                        if (chat.HostId == userId) { 
                            userResponse = await _httpClient.GetAsync($"https://localhost:44392/api/User/{chat.UserId}");
                        }
                        else
                        {
                            userResponse = await _httpClient.GetAsync($"https://localhost:44392/api/User/{chat.HostId}");
                        }
                        if (userResponse.IsSuccessStatusCode)
                        {
                            var userContent = await userResponse.Content.ReadAsStringAsync();
                            var user = JsonConvert.DeserializeObject<User>(userContent);

                            chatViewModel.ChatId = chat.Id;
                            chatViewModel.UserName = user.Name;
                            chatViewModel.UserLastName = user.LastName;
                            chatViewModel.UserImageBase64 = user.Picture; // Assuming user has an image stored as base64
                        }

                        // Get user places
                        var placesResponse = await _httpClient.GetAsync($"https://localhost:44392/api/Place/places/{chat.HostId}");
                        if (placesResponse.IsSuccessStatusCode)
                        {
                            var placesContent = await placesResponse.Content.ReadAsStringAsync();
                            var places = JsonConvert.DeserializeObject<IEnumerable<Place>>(placesContent);
                            chatViewModel.UserPlaces = places;
                        }

                        chatViewModels.Add(chatViewModel);
                    }

                    return View(chatViewModels);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve chats";
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
        public async Task<IActionResult> AddMessage(Message message)
        {
            try
            {
                ViewData["UserId"] = HttpContext.Session.GetInt32("UserId"); 
                ViewData["ChatId"] = HttpContext.Session.GetInt32("SelectedChatId");
                message.CreatedDate = DateTime.Now.ToString();
                var response = await _httpClient.PostAsJsonAsync("https://localhost:44392/api/Message", message);

                if (response.IsSuccessStatusCode)
                {
                    var chatId = message.ChatId;
                    var messages = await GetChatMessages(chatId);
                    return View("StartChat", messages); // Передаємо оновлений список повідомлень у вигляді моделі
                }
                else
                {
                    return BadRequest("Failed to add message");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartChat(string userEmail)
        {
            try
            {
                // Отримуємо ідентифікатор користувача за електронною поштою
                var getUserResponse = await _httpClient.GetAsync($"https://localhost:44392/api/User/GetUserIdByEmail/{userEmail}");

                if (getUserResponse.IsSuccessStatusCode)
                {
                    var userId = await getUserResponse.Content.ReadAsStringAsync();

                    // Створюємо чат з користувачем, якого потрібно знайти за електронною поштою
                    var chat = new Chat
                    {
                        UserId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId") ?? 0,
                        HostId = int.Parse(userId)
                    };

                    var jsonContent = System.Text.Json.JsonSerializer.Serialize(chat);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var createChatResponse = await _httpClient.PostAsJsonAsync("https://localhost:44392/api/Chat", content);


                    if (createChatResponse.IsSuccessStatusCode)
                    {
                        // Read the response content
                        var responseContent = await createChatResponse.Content.ReadAsStringAsync();

                        // Deserialize the chat object from the response content
                        var createdChat = System.Text.Json.JsonSerializer.Deserialize<Chat>(responseContent);

                        // Extract the chat ID
                        var chatId = createdChat.Id;

                        // Redirect to the view chat page with the chat ID
                        return RedirectToAction("ViewChat", new { chatId });
                    }
                    else
                    {
                        return BadRequest("Failed to create chat");
                    }
                }
                else
                {
                    return BadRequest("Failed to get user ID by email");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }


}
