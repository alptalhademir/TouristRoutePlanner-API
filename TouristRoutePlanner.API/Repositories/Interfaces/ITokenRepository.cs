using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(User user, List<string> roles);
    }
}
