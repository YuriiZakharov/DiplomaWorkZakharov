using DiplomaWork.Data;
using DiplomaWork.Interfaces;
using DiplomaWork.Models;

namespace DiplomaWork.Repisitory
{
    public class ImageRepository : IImageRepository
    {
        private readonly DataContext _context;
        public ImageRepository(DataContext context)
        {
            _context = context;
        }
        public bool AddImage(Image image)
        {
            _context.Images.Add(image);
            return Save();
        }

        public bool DeleteImage(int id)
        {
            var image = _context.Images.FirstOrDefault(x => x.Id == id);
            if (image != null)
                _context.Images.Remove(image);
            return Save();
        }

        public ICollection<Image> GetImages()
        {
            return _context.Images.OrderBy(x => x.Id).ToList();
        }

        public ICollection<Image> GetPlaceImages(int id)
        {
            return _context.Images.Where(x => x.PlaceId == id).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
