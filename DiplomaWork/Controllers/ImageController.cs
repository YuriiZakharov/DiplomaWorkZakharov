using DiplomaWork.Models;
using DiplomaWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DiplomaWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImageController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Image>> GetImages()
        {
            var images = _imageRepository.GetImages();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Image>> GetPlaceImages(int id)
        {
            var images = _imageRepository.GetPlaceImages(id);
            return Ok(images);
        }

        [HttpPost("add")]
        public ActionResult AddImage([FromBody] Image image)
        {
            if (_imageRepository.AddImage(image))
            {
                return Ok(image.Id);
            }
            return BadRequest();
        }

        [HttpDelete("delete/{id}")]
        public ActionResult DeleteImage(int id)
        {
            var image = _imageRepository.GetImages().FirstOrDefault(x => x.Id == id);
            if (image == null)
                return NotFound("Image not found");

            if (_imageRepository.DeleteImage(id))
                return Ok("Image deleted successfully");

            return BadRequest();

        }
    }
}
