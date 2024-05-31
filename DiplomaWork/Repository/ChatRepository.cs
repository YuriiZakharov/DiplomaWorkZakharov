using DiplomaWork.Data;
using DiplomaWork.Interfaces;
using DiplomaWork.Models;
using System.Security.Cryptography;
using System.Text;

namespace DiplomaWork.Repisitory
{
    public class ChatRepository : IChatRepository
    {
        private readonly DataContext _context;

        public ChatRepository(DataContext context)
        {
            _context = context;
        }

        public bool ChatExists(int id)
        {
            return _context.Chats.FirstOrDefault(c => c.Id == id) != null;
        }

        public bool DeleteChat(Chat chat)
        {
            _context.Remove(chat);
            foreach (var message in _context.Messages.ToList())
            {
                if (message.ChatId == chat.Id)
                    _context.Remove(message);
            }
            return Save();
        }

        public Chat GetChat(int id)
        {
            return _context.Chats.FirstOrDefault(c => c.Id == id);
        }

        public ICollection<Chat> GetChats(int userId)
        {
            return _context.Chats.Where(c => c.UserId == userId || c.HostId == userId).OrderBy(x => x.Id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool StartChat(Chat chat)
        {
           _context.Chats.Add(chat);
            return Save();
        }
    }
}
