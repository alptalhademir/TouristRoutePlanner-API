using Microsoft.EntityFrameworkCore;
using TouristRoutePlanner.API.Data;
using TouristRoutePlanner.API.Repositories.Interfaces;
using Type = TouristRoutePlanner.API.Models.Type;

namespace TouristRoutePlanner.API.Repositories.Implementations
{
    public class TypeRepository: ITypeRepository
    {
        private readonly TouristRoutePlannerDbContext dbContext;

        public TypeRepository(TouristRoutePlannerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Type>> GetAllAsync()
        {
            return await dbContext.Types.ToListAsync();
        }

        public async Task<Type?> GetByIdAsync(Guid id)
        {
            return await dbContext.Types
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Type> CreateAsync(Type type)
        {
            await dbContext.Types.AddAsync(type);
            await dbContext.SaveChangesAsync();
            return type;
        }

        public async Task<Type?> UpdateAsync(Guid id, Type type)
        {
            var existingType = await dbContext.Types
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingType == null) return null;

            existingType.Name = type.Name;

            await dbContext.SaveChangesAsync();
            return existingType;
        }

        public async Task<bool?> DeleteAsync(Guid id)
        {
            var existingType = await dbContext.Types
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingType == null) return null;

            dbContext.Types.Remove(existingType);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
