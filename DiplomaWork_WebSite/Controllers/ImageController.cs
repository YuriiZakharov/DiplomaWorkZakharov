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
    public class ImageController : Controller
    {
        private readonly ILogger<ImageController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int PlaceId = 0;

        public ImageController(ILogger<ImageController> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // Дія для відображення галереї зображень
        public async Task<IActionResult> Gallery(int placeId)
        {
            placeId = HttpContext.Session.GetInt32("PlaceId") ?? 0;
            ViewBag.PlaceId = placeId;
            var images = await GetImages(placeId);
            PlaceId = placeId;
            return View(images);
        }

        public async Task<IActionResult> GalleryNoEdit(int? placeId)
        {
            if (placeId.HasValue)
            {
                HttpContext.Session.SetInt32("PlaceId", placeId.Value);
            }
            else
            {
                placeId = HttpContext.Session.GetInt32("PlaceId") ?? 0;
            }

            ViewBag.PlaceId = placeId;
            var images = await GetImages(placeId.Value);
            return View(images);
        }

        // Метод для отримання зображень за ідентифікатором місця
        private async Task<List<Image>> GetImages(int placeId)
        {
            placeId = HttpContext.Session.GetInt32("PlaceId") ?? 0;
            var response = await _httpClient.GetAsync($"https://localhost:44392/api/Image/{placeId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Image>>();
            }
            else
            {
                throw new Exception("Failed to retrieve images");
            }
        }

        // Дія для додавання зображення
        [HttpPost]
        public async Task<IActionResult> AddImage(int placeId, IFormFile imageFile)
        {
            try
            {
                placeId = HttpContext.Session.GetInt32("PlaceId") ?? 0;
                // Перевірка, чи було завантажено зображення
                if (imageFile == null || imageFile.Length == 0)
                {
                    TempData["ErrorMessage"] = "No image uploaded";
                    return RedirectToAction("Gallery", new { placeId });
                }

                // Отримання рядка base64 з завантаженого зображення
                string imageCode;
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();
                    imageCode = Convert.ToBase64String(bytes);
                }

                // Створення об'єкта Image з рядком base64 та іншими даними
                var image = new Image { PlaceId = placeId, ImageCode = imageCode };

                // Відправка об'єкта Image на сервер
                var response = await _httpClient.PostAsJsonAsync("https://localhost:44392/api/Image/add", image);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Gallery", new { placeId });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add image";
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
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"https://localhost:44392/api/Image/delete/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Image deleted successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete image";
                }

                // Повертаємо користувача на попередню сторінку галереї
                return RedirectToAction("Gallery", new { placeId = PlaceId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index", "Home");
            }
        }



    }
}
