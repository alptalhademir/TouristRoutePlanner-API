using Microsoft.EntityFrameworkCore;
using TouristRoutePlanner.API.Data;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Repositories.Interfaces;

namespace TouristRoutePlanner.API.Repositories.Implementations
{
    public class TravelRepository : ITravelRepository
    {
        private readonly TouristRoutePlannerDbContext dbContext;

        public TravelRepository(TouristRoutePlannerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Travel>> GetAllAsync(Guid userId)
        {
            return await dbContext.Travels
                .Include(t => t.TravelTypes)
                    .ThenInclude(tt => tt.Type)
                .Include(t => t.TravelPlaces)
                    .ThenInclude(tp => tp.Place)
                        .ThenInclude(p => p.PlaceTypes)
                            .ThenInclude(pt => pt.Type)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<Travel?> GetByIdAsync(Guid id, Guid userId)
        {
            return await dbContext.Travels
                .Include(t => t.TravelTypes)
                    .ThenInclude(tt => tt.Type)
                .Include(t => t.TravelPlaces)
                    .ThenInclude(tp => tp.Place)
                        .ThenInclude(p => p.PlaceTypes)
                            .ThenInclude(pt => pt.Type)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<Travel?> GetByIdWithPlacesAsync(Guid travelId, Guid userId)
        {
            return await dbContext.Travels
                .Include(t => t.TravelPlaces)
                    .ThenInclude(tp => tp.Place)
                .FirstOrDefaultAsync(t => t.Id == travelId && t.UserId == userId);
        }

        public async Task<Travel> CreateAsync(Guid userId, Travel travel)
        {
            travel.UserId = userId;

            // Create TravelTypes
            var travelTypes = new List<TravelType>();
            foreach (var travelType in travel.TravelTypes.ToList())
            {
                travel.TravelTypes.Clear();

                var existingType = await dbContext.Types
                    .FirstOrDefaultAsync(t => t.Id == travelType.TypeId);

                if (existingType == null)
                    throw new InvalidOperationException($"Type with ID {travelType.TypeId} does not exist");

                travelTypes.Add(new TravelType
                {
                    TravelId = travel.Id,
                    TypeId = travelType.TypeId
                });
            }

            // Create TravelPlaces
            var travelPlaces = new List<TravelPlace>();
            foreach (var travelPlace in travel.TravelPlaces.ToList())
            {
                travel.TravelPlaces.Clear();

                var existingPlace = await dbContext.Places
                    .FirstOrDefaultAsync(p => p.Id == travelPlace.PlaceId);

                if (existingPlace == null)
                    throw new InvalidOperationException($"Place with ID {travelPlace.PlaceId} does not exist");

                travelPlaces.Add(new TravelPlace
                {
                    TravelId = travel.Id,
                    PlaceId = travelPlace.PlaceId
                });
            }

            // Add everything to context
            await dbContext.Travels.AddAsync(travel);
            await dbContext.TravelTypes.AddRangeAsync(travelTypes);
            await dbContext.TravelPlaces.AddRangeAsync(travelPlaces);

            await dbContext.SaveChangesAsync();

            // Return the complete travel with related entities
            return await GetByIdAsync(travel.Id, travel.UserId);
        }

        public async Task<Travel?> UpdateAsync(Guid id, Guid userId, Travel travel)
        {
            var existingTravel = await dbContext.Travels
                .Include(t => t.TravelTypes)
                .Include(t => t.TravelPlaces)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (existingTravel == null)
                return null;

            // Update basic properties
            existingTravel.StartDate = travel.StartDate;
            existingTravel.EndDate = travel.EndDate;
            existingTravel.City = travel.City;
            existingTravel.Title = travel.Title;
            existingTravel.TravelerType = travel.TravelerType;

            // Update TravelTypes
            dbContext.TravelTypes.RemoveRange(existingTravel.TravelTypes);
            var newTravelTypes = new List<TravelType>();
            foreach (var travelType in travel.TravelTypes)
            {
                var existingType = await dbContext.Types
                    .FirstOrDefaultAsync(t => t.Id == travelType.TypeId);

                if (existingType == null)
                    throw new InvalidOperationException($"Type with ID {travelType.TypeId} does not exist");

                newTravelTypes.Add(new TravelType
                {
                    TravelId = existingTravel.Id,
                    TypeId = travelType.TypeId
                });
            }

            // Update TravelPlaces
            dbContext.TravelPlaces.RemoveRange(existingTravel.TravelPlaces);
            var newTravelPlaces = new List<TravelPlace>();
            foreach (var travelPlace in travel.TravelPlaces)
            {
                var existingPlace = await dbContext.Places
                    .FirstOrDefaultAsync(p => p.Id == travelPlace.PlaceId);

                if (existingPlace == null)
                    throw new InvalidOperationException($"Place with ID {travelPlace.PlaceId} does not exist");

                newTravelPlaces.Add(new TravelPlace
                {
                    TravelId = existingTravel.Id,
                    PlaceId = travelPlace.PlaceId
                });
            }

            existingTravel.TravelTypes = newTravelTypes;
            existingTravel.TravelPlaces = newTravelPlaces;

            await dbContext.SaveChangesAsync();

            return await GetByIdAsync(id, userId);
        }

        public async Task<bool?> DeleteAsync(Guid id, Guid userId)
        {
            var existingTravel = await dbContext.Travels
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (existingTravel == null)
                return null;

            dbContext.Travels.Remove(existingTravel);

            try
            {
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
