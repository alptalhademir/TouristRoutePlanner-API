using TouristRoutePlanner.API.DTOs;
using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Services.Implementations
{
    public interface IRouteOptimizationService
    {
        Task<List<RouteOptimizationResponseDto>> OptimizeRouteAsync(Guid travelId, Guid userId, OptimizationConstraints constraints);
    }
}
