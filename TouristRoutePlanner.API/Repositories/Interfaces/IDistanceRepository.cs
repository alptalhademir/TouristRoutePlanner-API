using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Repositories.Interfaces
{
    public interface IDistanceRepository
    {
        Task<List<Distance>> GetAllAsync();
        Task<Distance?> GetByIdAsync(Guid id);
        Task<Distance> CreateAsync(Distance distance);
        Task<Distance?> UpdateAsync(Guid id, Distance distance);
        Task<bool?> DeleteAsync(Guid id);
        Task<Distance?> GetDistanceBetweenPlacesAsync(string originExternalId,
            string destinationExternalId);
        Task<List<Distance>> GetDistancesForPlaceAsync(string placeExternalId);
    }
}
