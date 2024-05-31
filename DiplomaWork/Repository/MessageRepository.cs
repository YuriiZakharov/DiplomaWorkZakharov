using DiplomaWork.Data;
using DiplomaWork.Interfaces;
using DiplomaWork.Models;
using System.Security.Cryptography;
using System.Text;

namespace DiplomaWork.Repisitory
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        public MessageRepository(DataContext context)
        {
            _context = context;
        }

        public bool AddMessage(Message message)
        {
           _context.Messages.Add(message);
            return Save();
        }

        public bool DeleteMessage(Message message)
        {
            _context.Remove(message);
            return Save();
        }

        public ICollection<Message> GetChatMessages(int chatId)
        {
            return _context.Messages.Where(m => m.ChatId == chatId).ToList();
        }

        public Message GetMessage(int id)
        {
            return _context.Messages.FirstOrDefault(m => m.Id == id);
        }

        public Message GetMessageById(int id)
        {
            return _context.Messages.FirstOrDefault(m => m.Id == id);
        }

        public ICollection<Message> GetUserMessages(int userId)
        {
            return _context.Messages.Where(m => m.UserId == userId).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateMessage(Message message)
        {
            _context.Update(message);
            return Save();
        }
    }
}
