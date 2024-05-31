using DiplomaWork.Models;
using DiplomaWork.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections;

namespace DiplomaWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceRepository _placeRepository;

        public PlaceController(IPlaceRepository placeRepository)
        {
            _placeRepository = placeRepository;
        }

        [HttpGet("places")]
        public ActionResult<IEnumerable<Place>> GetPlaces()
        {
            var places = _placeRepository.GetPlaces();
            return Ok(places);
        }

        [HttpGet("{id}")]
        public ActionResult<Place> GetPlace(int id)
        {
            var place = _placeRepository.GetPlaceById(id);
            if (place == null)
            {
                return NotFound("Place not found");
            }
            return Ok(place);
        }

        [HttpGet("places/{id}")]
        public ActionResult<IEnumerable<Place>> GetUserPlaces(int id)
        {
            var places = _placeRepository.GetPlaces();
            var result = new List<Place>();
            foreach (Place place in places)
            {
                if(place.HostId == id)
                    result.Add(place);                    
            }
            return result;
        }

        [HttpPost]
        public ActionResult AddPlace([FromBody] Place place)
        {
            _placeRepository.AddPlace(place);
            {
                return Ok("Place added successfully");
            }
            return BadRequest("Failed to add place");
        }

        [HttpPut("place/update")]
        public ActionResult UpdatePlace([FromBody] Place place)
        {
            try
            {
                var existingPlace = _placeRepository.GetPlaceById(place.Id);

                if (existingPlace == null)
                {
                    return NotFound("Place not found");
                }

                existingPlace.Name = place.Name;
                existingPlace.ProductType = place.ProductType;                
                existingPlace.Address = place.Address;
                existingPlace.WorkingHours = place.WorkingHours;
                existingPlace.HostId = place.HostId;
                existingPlace.OwnershipForm = place.OwnershipForm;

                _placeRepository.UpdatePlace(existingPlace);

                return Ok("Place updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to update place");
            }
        }

        [HttpDelete("delete/{id}")]
        public ActionResult DeletePlace(int id)
        {
            var place = _placeRepository.GetPlaceById(id);
            if (place == null)
            {
                return NotFound("Place not found");
            }

            if (_placeRepository.DeletePlace(place))
            {
                return Ok("Place deleted successfully");
            }

            return BadRequest("Failed to delete place");
        }

        [HttpGet("byHour")]
        public ActionResult<IEnumerable<Place>> GetPlacesByHour([FromQuery] string startTime, [FromQuery] string endTime)
        {
            try
            {
                var places = _placeRepository.GetPlacesByHour(startTime, endTime);
                return Ok(places);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid time format. Use HH:mm");
            }
        }        
    }

    public class ImageRequest
    {
        public string Image { get; set; }
    }
}