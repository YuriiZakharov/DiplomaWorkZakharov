using DiplomaWork.Models;

namespace DiplomaWork.Interfaces
{
    public interface IMessageRepository
    {
        ICollection<Message> GetChatMessages(int chatId);
        ICollection<Message> GetUserMessages(int userId);
        Message GetMessage(int id);
        Message GetMessageById(int id);
        bool AddMessage(Message message);
        bool UpdateMessage(Message message); 
        bool DeleteMessage(Message message);
        bool Save();
    }
}
