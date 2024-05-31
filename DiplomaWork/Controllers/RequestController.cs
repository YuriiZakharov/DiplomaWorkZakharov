using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DiplomaWork.Data;
using DiplomaWork.Models;
using DiplomaWork.Interfaces;

namespace DiplomaWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IRequestRepository _requestRepository;

        public RequestController(DataContext context, IRequestRepository requestRepository)
        {
            _context = context;
            _requestRepository = requestRepository;
        }

        [HttpGet("user/requests")]
        public async Task<IEnumerable<Request>> GetUserRequests(int userId)
        {
            var requests = _requestRepository.GetUserRequests(userId);
            return requests;
        }

        [HttpGet("place/requests")]
        public async Task<IEnumerable<Request>> GetPlaceRequests(int placeId)
        {
            var requests = _requestRepository.GetPlaceRequests(placeId);
            return requests;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRequest(Request request)
        {
            _requestRepository.AddRequest(request);
            return Ok();
        }

        [HttpPut("update/request")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var request = _requestRepository.GetRequest(requestId);
            request.IsAccepted = true;
            _requestRepository.UpdateRequest(request);
            await Task.Delay(6 * 3600 * 1000);
            _requestRepository.DeleteRequest(request); ;
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id, string text)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Id == id);
            request.IsDeclined = true;
            request.DeclineText = text;
            _requestRepository.UpdateRequest(request);
            await Task.Delay(6 * 3600 * 1000); ;            
            if (request == null)
            {
                return NotFound();
            }

            _requestRepository.DeleteRequest(request);
            return Ok();
        }

        [HttpDelete("deleteNow/{id}")]
        public async Task<IActionResult> DeleteRequestNow(int id)
        {            
            var request = _context.Requests.FirstOrDefault(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            _requestRepository.DeleteRequest(request);
            return Ok();
        }


    }
}
