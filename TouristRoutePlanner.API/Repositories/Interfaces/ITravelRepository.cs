using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Repositories.Interfaces
{
    public interface ITravelRepository
    {
        Task<List<Travel>> GetAllAsync(Guid userId);
        Task<Travel?> GetByIdAsync(Guid id, Guid userId);
        Task<Travel> CreateAsync(Guid userId, Travel travel);
        Task<Travel?> UpdateAsync(Guid id, Guid userId, Travel travel);
        Task<bool?> DeleteAsync(Guid id, Guid userId);
    }
}
