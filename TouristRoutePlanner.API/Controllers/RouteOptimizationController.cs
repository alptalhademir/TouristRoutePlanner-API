using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Services.Implementations;

namespace TouristRoutePlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RouteOptimizationController : ControllerBase
    {
        private readonly IRouteOptimizationService routeOptimizationService;

        public RouteOptimizationController(IRouteOptimizationService routeOptimizationService)
        {
            this.routeOptimizationService = routeOptimizationService;
        }

        [HttpPost]
        [Route("{travelId:guid}/optimize")]
        public async Task<IActionResult> OptimizeRoute([FromRoute] Guid travelId, 
            [FromBody] OptimizationConstraints constraints)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Please login to proceed." });

            if (constraints == null)
                return BadRequest(new { message = "Constraints cannot be null." });
            var optimizedRoute = await routeOptimizationService.OptimizeRouteAsync(travelId, 
                Guid.Parse(userId), constraints);
            if (optimizedRoute == null)
                return NotFound(new { message = "No optimized route found." });
            return Ok(optimizedRoute);
        }
    }
}
