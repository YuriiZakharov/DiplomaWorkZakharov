using DiplomaWork.Models;

namespace DiplomaWork.Interfaces
{
    public interface IPlaceRepository
    {
        ICollection<Place> GetPlaces();
        Place GetPlaceById(int id);
        Place GetPlaceByName(string name);
        Place GetPlaceByAddress(string address);
        ICollection<Place> GetPlacesByHour(string startTime, string endTime);        
        bool PlaceExists(int id);
        bool AddPlace(Place place);        
        bool UpdatePlace(Place place);
        bool DeletePlace(Place place);
        bool Save();
    }
}
