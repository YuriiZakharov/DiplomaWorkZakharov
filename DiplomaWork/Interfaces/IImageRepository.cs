using DiplomaWork.Models;

namespace DiplomaWork.Interfaces
{
    public interface IImageRepository
    {
        ICollection<Image> GetImages();
        ICollection<Image> GetPlaceImages(int id);
        bool AddImage(Image image);
        bool DeleteImage(int id);
        bool Save();
    }
}
