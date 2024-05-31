using DiplomaWork.Models;

namespace DiplomaWork.Interfaces
{
    public interface IRequestRepository
    {
        ICollection<Request> GetUserRequests(int userId);
        ICollection<Request> GetPlaceRequests(int placeId);
        Request GetRequest(int id);
        bool RequestExists(int id);
        bool AddRequest(Request request);        
        bool UpdateRequest(Request request);
        bool DeleteRequest(Request request);
        bool Save();
    }
}
