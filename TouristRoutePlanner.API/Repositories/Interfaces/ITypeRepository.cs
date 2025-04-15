using Type = TouristRoutePlanner.API.Models.Type;

namespace TouristRoutePlanner.API.Repositories.Interfaces
{
    public interface ITypeRepository
    {
        Task<List<Type>> GetAllAsync();
        Task<Type?> GetByIdAsync(Guid id);
        Task<Type> CreateAsync(Type type);
        Task<Type?> UpdateAsync(Guid id, Type type);
        Task<bool?> DeleteAsync(Guid id);
    }
}
