using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TouristRoutePlanner.API.CustomActionFilters;
using TouristRoutePlanner.API.DTOs;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Repositories.Interfaces;

namespace TouristRoutePlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlacesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IPlaceRepository placeRepository;

        public PlacesController(IMapper mapper, IPlaceRepository placeRepository)
        {
            this.mapper = mapper;
            this.placeRepository = placeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var places = await placeRepository.GetAllAsync();

            return Ok(mapper.Map<List<PlaceDto>>(places));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var place = await placeRepository.GetByIdAsync(id);

            if (place == null) return NotFound();

            return Ok(mapper.Map<PlaceDto>(place));
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AddPlaceRequestDto addPlaceRequestDto)
        {
            var place = mapper.Map<Place>(addPlaceRequestDto);

            place = await placeRepository.CreateAsync(place);

            return Ok(mapper.Map<PlaceDto>(place));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePlaceRequestDto updatePlaceRequestDto)
        {
            var place = mapper.Map<Place>(updatePlaceRequestDto);

            place = await placeRepository.UpdateAsync(id, place);

            if (place == null) return NotFound();

            return Ok(mapper.Map<PlaceDto>(place));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var isPlaceDeleted = await placeRepository.DeleteAsync(id);

            if (isPlaceDeleted == null) return NotFound();

            return Ok("Place deleted successfully");
        }
    }
}
