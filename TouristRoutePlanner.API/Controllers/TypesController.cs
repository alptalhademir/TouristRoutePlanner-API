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
    public class TypesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITypeRepository typeRepository;

        public TypesController(IMapper mapper, ITypeRepository typeRepository)
        {
            this.mapper = mapper;
            this.typeRepository = typeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var types = await typeRepository.GetAllAsync();
            return Ok(mapper.Map<List<TypeDto>>(types));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var type = await typeRepository.GetByIdAsync(id);

            if (type == null) return NotFound();

            return Ok(mapper.Map<TypeDto>(type));
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create([FromBody] AddTypeRequestDto addTypeRequestDto)
        {
            var type = mapper.Map<Models.Type>(addTypeRequestDto);

            type = await typeRepository.CreateAsync(type);

            return Ok(mapper.Map<TypeDto>(type));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id,
            [FromBody] UpdateTypeRequestDto updateTypeRequestDto)
        {
            var type = mapper.Map<Models.Type>(updateTypeRequestDto);

            type = await typeRepository.UpdateAsync(id, type);

            if (type == null) return NotFound();

            return Ok(mapper.Map<TypeDto>(type));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await typeRepository.DeleteAsync(id);

            if (result == null) return NotFound();

            return Ok("Type deleted successfully");
        }
    }
}
