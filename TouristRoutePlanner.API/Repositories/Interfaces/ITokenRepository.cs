using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(User user, List<string> roles);

        Task<string> CreateRefreshTokenAsync(Guid userId);
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
        Task<string> RefreshTokenAsync(Guid userId, string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
