using Microsoft.EntityFrameworkCore;
using TouristRoutePlanner.API.Data;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Repositories.Interfaces;

namespace TouristRoutePlanner.API.Repositories.Implementations
{
    public class DistanceRepository : IDistanceRepository
    {
        private readonly TouristRoutePlannerDbContext dbContext;

        public DistanceRepository(TouristRoutePlannerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Distance>> GetAllAsync()
        {
            return await dbContext.Distances.ToListAsync();
        }

        public async Task<Distance?> GetByIdAsync(Guid id)
        {
            return await dbContext.Distances
                .Include(d => d.OriginPlace)
                .Include(d => d.DestinationPlace)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Distance> CreateAsync(Distance distance)
        {
            var originExists = await dbContext.Places
                .AnyAsync(p => p.ExternalId == distance.OriginPlaceExternalId);
            var destinationExists = await dbContext.Places
                .AnyAsync(p => p.ExternalId == distance.DestinationPlaceExternalId);

            if (!originExists || !destinationExists)
            {
                throw new InvalidOperationException("Origin or destination place does not exist");
            }

            await dbContext.Distances.AddAsync(distance);
            await dbContext.SaveChangesAsync();
            return distance;
        }

        public async Task<Distance?> UpdateAsync(Guid id, Distance distance)
        {
            var existingDistance = await dbContext.Distances
                .FirstOrDefaultAsync(d => d.Id == id);

            if (existingDistance == null) return null;

            existingDistance.WalkingDistance = distance.WalkingDistance;
            existingDistance.WalkingDuration = distance.WalkingDuration;
            existingDistance.DrivingDistance = distance.DrivingDistance;
            existingDistance.DrivingDuration = distance.DrivingDuration;

            await dbContext.SaveChangesAsync();

            return existingDistance;
        }

        public async Task<bool?> DeleteAsync(Guid id)
        {
            var existingDistance = await dbContext.Distances
                .FirstOrDefaultAsync(d => d.Id == id);

            if (existingDistance == null) return null;

            dbContext.Distances.Remove(existingDistance);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Distance?> GetDistanceBetweenPlacesAsync(string originExternalId, 
            string destinationExternalId)
        {
            return await dbContext.Distances
                .Include(d => d.OriginPlace)
                .Include(d => d.DestinationPlace)
                .FirstOrDefaultAsync(d =>
                    d.OriginPlaceExternalId == originExternalId &&
                    d.DestinationPlaceExternalId == destinationExternalId);
        }

        public async Task<List<Distance>> GetDistancesForPlaceAsync(string placeExternalId)
        {
            return await dbContext.Distances
                .Include(d => d.OriginPlace)
                .Include(d => d.DestinationPlace)
                .Where(d =>
                    d.OriginPlaceExternalId == placeExternalId ||
                    d.DestinationPlaceExternalId == placeExternalId)
                .ToListAsync();
        }
    }
}
