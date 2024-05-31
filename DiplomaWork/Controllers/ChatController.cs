using DiplomaWork.Models;
using DiplomaWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DiplomaWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;

        public ChatController(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        [HttpGet("{id}")]
        public ActionResult<Chat> GetChat(int id)
        {
            var chat = _chatRepository.GetChat(id);
            if (chat == null)
            {
                return NotFound("Chat not found");
            }
            return Ok(chat);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Chat>> GetChats(int userId)
        {
            var chats = _chatRepository.GetChats(userId);
            return Ok(chats);
        }

        [HttpPost]
        public ActionResult StartChat([FromBody] Chat chat)
        {
            if (_chatRepository.StartChat(chat))
            {
                return Ok(chat);
            }
            return BadRequest("Failed to start chat");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteChat(int id)
        {
            var chat = _chatRepository.GetChat(id);
            if (chat == null)
            {
                return NotFound("Chat not found");
            }

            if (_chatRepository.DeleteChat(chat))
            {
                return Ok("Chat deleted successfully");
            }

            return BadRequest("Failed to delete chat");
        }
    }
}