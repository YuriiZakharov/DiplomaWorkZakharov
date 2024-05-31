using DiplomaWork.Data;
using DiplomaWork.Interfaces;
using DiplomaWork.Models;

namespace DiplomaWork.Repisitory
{
    public class RequestRepository : IRequestRepository
    {
        private readonly DataContext _context;

        public RequestRepository(DataContext context)
        {
            _context = context;
        }

        public Request GetRequest(int id)
        {
            return _context.Requests.FirstOrDefault(x => x.Id == id);
        }

        public bool AddRequest(Request request)
        {
            _context.Requests.Add(request);
            return Save();
        }

        public bool UpdateRequest(Request request)
        {
            _context.Requests.Update(request);
            return Save();
        }

        public bool DeleteRequest(Request request)
        {
            _context.Requests.Remove(request);
            return Save();
        }

        public ICollection<Request> GetPlaceRequests(int placeId)
        {
            return _context.Requests.Where(x => x.PlaceId == placeId).ToList();
        }

        public ICollection<Request> GetUserRequests(int userId)
        {
            return _context.Requests.Where(x => x.UserId == userId).ToList();
        }

        public bool RequestExists(int id)
        {
            return _context.Requests.FirstOrDefault(r => r.Id == id) != null;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
