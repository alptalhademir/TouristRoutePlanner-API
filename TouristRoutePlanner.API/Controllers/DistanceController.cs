using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouristRoutePlanner.API.CustomActionFilters;
using TouristRoutePlanner.API.DTOs;
using TouristRoutePlanner.API.Repositories.Interfaces;

namespace TouristRoutePlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DistanceController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IDistanceRepository distanceRepository;

        public DistanceController(IMapper mapper, IDistanceRepository distanceRepository)
        {
            this.mapper = mapper;
            this.distanceRepository = distanceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var distances = await distanceRepository.GetAllAsync();
            return Ok(mapper.Map<List<DistanceDto>>(distances));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var distance = await distanceRepository.GetByIdAsync(id);
            if (distance == null) return NotFound();
            return Ok(mapper.Map<DistanceDto>(distance));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddDistanceRequestDto addDistanceRequestDto)
        {
            var distance = mapper.Map<Models.Distance>(addDistanceRequestDto);
            distance = await distanceRepository.CreateAsync(distance);
            return Ok(mapper.Map<DistanceDto>(distance));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id,
            [FromBody] UpdateDistanceRequestDto updateDistanceRequestDto)
        {
            var distance = mapper.Map<Models.Distance>(updateDistanceRequestDto);
            var updatedDistance = await distanceRepository.UpdateAsync(id, distance);
            if (updatedDistance == null) return NotFound();
            return Ok(mapper.Map<DistanceDto>(updatedDistance));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await distanceRepository.DeleteAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        [Route("origin/{originId}/destination/{destinationId}")]
        public async Task<IActionResult> GetDistanceByOriginAndDestination([FromRoute] string originId, [FromRoute] string destinationId)
        {
            var distance = await distanceRepository.GetDistanceBetweenPlacesAsync(originId, destinationId);
            if (distance == null) return NotFound();
            return Ok(mapper.Map<DistanceDto>(distance));
        }

        [HttpGet]
        [Route("place/{placeId}")]
        public async Task<IActionResult> GetDistancesForPlace([FromRoute] string placeId)
        {
            var distances = await distanceRepository.GetDistancesForPlaceAsync(placeId);
            if (distances == null) return NotFound();
            return Ok(mapper.Map<List<DistanceDto>>(distances));
        }


    }
}
