using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TouristRoutePlanner.API.CustomActionFilters;
using TouristRoutePlanner.API.DTOs;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Repositories.Interfaces;

namespace TouristRoutePlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TravelsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITravelRepository travelRepository;

        public TravelsController(IMapper mapper, ITravelRepository travelRepository)
        {
            this.mapper = mapper;
            this.travelRepository = travelRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Please login to proceed.");
            var travels = await travelRepository.GetAllAsync(Guid.Parse(userId));
            return Ok(mapper.Map<List<TravelDto>>(travels));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Please login to proceed.");
            var travel = await travelRepository.GetByIdAsync(id, Guid.Parse(userId));
            if (travel == null) return NotFound();
            return Ok(mapper.Map<TravelDto>(travel));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddTravelRequestDto addTravelRequestDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized("Please login to proceed.");
            var travel = mapper.Map<Travel>(addTravelRequestDto);
            travel = await travelRepository.CreateAsync(Guid.Parse(userId), travel);
            return Ok(mapper.Map<TravelDto>(travel));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTravelRequestDto updateTravelRequestDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Please login to create a travel plan.");
            var travel = mapper.Map<Travel>(updateTravelRequestDto);
            travel = await travelRepository.UpdateAsync(id, Guid.Parse(userId), travel);
            if (travel == null) return NotFound();
            return Ok(mapper.Map<TravelDto>(travel));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Please login to proceed.");
            var result = await travelRepository.DeleteAsync(id, Guid.Parse(userId));
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
