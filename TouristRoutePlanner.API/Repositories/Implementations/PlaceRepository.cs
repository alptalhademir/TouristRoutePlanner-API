using Microsoft.EntityFrameworkCore;
using TouristRoutePlanner.API.Data;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Repositories.Interfaces;

namespace TouristRoutePlanner.API.Repositories.Implementations
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly TouristRoutePlannerDbContext dbContext;

        public PlaceRepository(TouristRoutePlannerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Place>> GetAllAsync()
        {
            return await dbContext.Places
                .Include(p => p.PlaceTypes)
                .ThenInclude(pt => pt.Type)
                .ToListAsync();
        }

        public async Task<Place?> GetByIdAsync(Guid id)
        {
            return await dbContext.Places
                .Include(p => p.PlaceTypes)
                .ThenInclude(pt => pt.Type)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Place> CreateAsync(Place place)
        {
            var typeNames = place.PlaceTypes.Select(pt => pt.Type.Name).ToList();
            var existingTypes = await dbContext.Types
                .Where(t => typeNames.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name, t => t);

            var placeTypes = new List<PlaceType>();
            foreach (var placeType in place.PlaceTypes.ToList())
            {
                place.PlaceTypes.Clear();

                // Check if the type already exists
                if (existingTypes.TryGetValue(placeType.Type.Name, out var existingType))
                {
                    placeTypes.Add(new PlaceType
                    {
                        Place = place,
                        TypeId = existingType.Id
                    });
                }

                // If the type does not exist, create a new one
                else
                {
                    var newType = new Models.Type
                    {
                        Id = Guid.NewGuid(),
                        Name = placeType.Type.Name
                    };
                    await dbContext.Types.AddAsync(newType);

                    placeTypes.Add(new PlaceType
                    {
                        Place = place,
                        Type = newType
                    });

                }
            }

            await dbContext.Places.AddAsync(place);

            await dbContext.PlaceTypes.AddRangeAsync(placeTypes);

            await dbContext.SaveChangesAsync();

            return await dbContext.Places
                .Include(p => p.PlaceTypes)
                .ThenInclude(pt => pt.Type)
                .FirstAsync(p => p.Id == place.Id);
        }
    
        public async Task<Place?> UpdateAsync(Guid id, Place place)
        {
            var existingPlace = await dbContext.Places
                .Include(p => p.PlaceTypes)
                .ThenInclude(pt => pt.Type)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingPlace == null)
            {
                return null;
            }

            existingPlace.DisplayName = place.DisplayName;
            existingPlace.LanguageCode = place.LanguageCode;
            existingPlace.City = place.City;
            existingPlace.Rating = place.Rating;
            existingPlace.PriceLevel = place.PriceLevel;
            existingPlace.Latitude = place.Latitude;
            existingPlace.Longitude = place.Longitude;

            var newTypeNames = place.PlaceTypes.Select(pt => pt.Type.Name).ToList();

            var existingTypes = await dbContext.Types
                .Where(t => newTypeNames.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name, t => t);

            dbContext.PlaceTypes.RemoveRange(existingPlace.PlaceTypes);

            var newPlaceTypes = new List<PlaceType>();
            foreach (var placeType in place.PlaceTypes)
            {
                // Check if type exists in database
                if (existingTypes.TryGetValue(placeType.Type.Name, out var existingType))
                {
                    newPlaceTypes.Add(new PlaceType
                    {
                        PlaceId = existingPlace.Id,
                        TypeId = existingType.Id
                    });
                }

                // If the type does not exist, create a new one
                else
                {
                    var newType = new Models.Type
                    {
                        Id = Guid.NewGuid(),
                        Name = placeType.Type.Name
                    };
                    await dbContext.Types.AddAsync(newType);

                    newPlaceTypes.Add(new PlaceType
                    {
                        PlaceId = existingPlace.Id,
                        Type = newType
                    });
                }
            }

            existingPlace.PlaceTypes = newPlaceTypes;

            await dbContext.SaveChangesAsync();

            return await dbContext.Places
                .Include(p => p.PlaceTypes)
                .ThenInclude(pt => pt.Type)
                .FirstAsync(p => p.Id == id);
        }

        public async Task<bool?> DeleteAsync(Guid id)
        {
            // Get the place with its related entities
            var existingPlace = await dbContext.Places
                .Include(p => p.PlaceTypes)
                .Include(p => p.TravelPlaces)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingPlace == null) return null;

            var relatedDistances = await dbContext.Distances
                .Where(d => d.OriginPlaceExternalId == existingPlace.ExternalId ||
                            d.DestinationPlaceExternalId == existingPlace.ExternalId)
                .ToListAsync();

            // Remove related distances
            if (relatedDistances.Any())
            {
                dbContext.Distances.RemoveRange(relatedDistances);
            }

            dbContext.Places.Remove(existingPlace);

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
