using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Repositories.Interfaces
{
    public interface IPlaceRepository
    {
        Task<List<Place>> GetAllAsync();
        Task<Place?> GetByIdAsync(Guid id);
        Task<Place> CreateAsync(Place place);
        Task<Place?> UpdateAsync(Guid id, Place place);
        Task<bool?> DeleteAsync(Guid id);
    }
}
