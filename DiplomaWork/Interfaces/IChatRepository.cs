using DiplomaWork.Models;

namespace DiplomaWork.Interfaces
{
    public interface IChatRepository
    {
        Chat GetChat(int id);
        ICollection<Chat> GetChats(int userId);
        bool ChatExists(int id);
        bool StartChat(Chat chat);        
        bool DeleteChat(Chat chat);
        bool Save();
    }
}
