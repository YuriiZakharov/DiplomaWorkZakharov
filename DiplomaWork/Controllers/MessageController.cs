using DiplomaWork.Models;
using DiplomaWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DiplomaWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpPost]
        public ActionResult AddMessage([FromBody] Message message)
        {
            if (_messageRepository.AddMessage(message))
            {
                return Ok(message);
            }
            return BadRequest("Failed to add message");
        }

        [HttpPut("message/update")]
        public ActionResult UpdateMessage([FromBody] Message message)
        {
            try
            {
                var existingMessage = _messageRepository.GetMessageById(message.Id);

                if (existingMessage == null)
                {
                    return NotFound("Message not found");
                }

                existingMessage.Text = message.Text;
                existingMessage.CreatedDate = message.CreatedDate;

                _messageRepository.UpdateMessage(existingMessage);

                return Ok("Message updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to update message");
            }
        }

        [HttpGet("chat/{chatId}")]
        public ActionResult<IEnumerable<Message>> GetChatMessages(int chatId)
        {
            var messages = _messageRepository.GetChatMessages(chatId);
            return Ok(messages);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Message>> GetUserMessages(int userId)
        {
            var messages = _messageRepository.GetUserMessages(userId);
            return Ok(messages);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteMessage(int id)
        {
            var message = _messageRepository.GetMessage(id);
            if (message == null)
            {
                return NotFound("Message not found");
            }

            if (_messageRepository.DeleteMessage(message))
            {
                return Ok("Message deleted successfully");
            }

            return BadRequest("Failed to delete message");
        }
    }
}