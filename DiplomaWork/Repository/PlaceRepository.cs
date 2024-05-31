using DiplomaWork.Data;
using DiplomaWork.Interfaces;
using DiplomaWork.Models;
using Microsoft.EntityFrameworkCore;

namespace DiplomaWork.Repisitory
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly DataContext _context;

        public PlaceRepository(DataContext context)
        {
            _context = context;
        }               

        public bool AddPlace(Place place)
        {            
            if (_context.Users.FirstOrDefault(x => x.Id == place.HostId) != null)            
                _context.Places.Add(place);
            return Save();
        }

        public bool DeletePlace(Place place)
        {                     
            _context.Remove(place);
             return Save();
        }

        public Place GetPlaceByAddress(string address)
        {
            return _context.Places.FirstOrDefault(p => p.Address == address);
        }

        public Place GetPlaceById(int id)
        {
            return _context.Places.FirstOrDefault(x => x.Id == id);
        }

        public Place GetPlaceByName(string name)
        {
            return _context.Places.FirstOrDefault(p => p.Name.Contains(name));
        }

        public ICollection<Place> GetPlaces()
        {
            return _context.Places.OrderBy(p => p.Id).ToList();
        }

        public ICollection<Place> GetPlacesByHour(string startTime, string endTime)
        {
            TimeSpan start = TimeSpan.Parse(startTime);
            TimeSpan end = TimeSpan.Parse(endTime);

            List<Place> placesInTimeRange = _context.Places
                .Where(place => EF.Functions.Like(place.WorkingHours, $"%{startTime}-{endTime}%"))
                .ToList();

            return placesInTimeRange;
        }
        

        public bool PlaceExists(int id)
        {
            return _context.Places.FirstOrDefault(p => p.Id == id) != null;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePlace(Place place)
        {
            _context.Update(place);
            return Save();
        }

        private bool IsPlaceOpenInTimeRange(string workingHours, TimeSpan startTime, TimeSpan endTime)
        {            
            string[] hoursArray = workingHours.Split('-');
            if (hoursArray.Length != 2)
            {
                return false;
            }

            TimeSpan openingTime = TimeSpan.Parse(hoursArray[0]);
            TimeSpan closingTime = TimeSpan.Parse(hoursArray[1]);
            
            return startTime >= openingTime && endTime <= closingTime;
        }
    }
}
