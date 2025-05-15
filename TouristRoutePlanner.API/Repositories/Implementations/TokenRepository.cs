using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TouristRoutePlanner.API.Data;
using TouristRoutePlanner.API.Models;
using TouristRoutePlanner.API.Repositories.Interfaces;

namespace TouristRoutePlanner.API.Repositories.Implementations
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;
        private readonly TouristRoutePlannerAuthDbContext authDbContext;

        public TokenRepository(IConfiguration configuration, TouristRoutePlannerAuthDbContext authDbContext)
        {
            this.configuration = configuration;
            this.authDbContext = authDbContext;
        }

        public string CreateJWTToken(User user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshTokenAsync(Guid userId)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);

            var expiryTime = DateTime.UtcNow.AddDays(7);

            var token = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiryTime = expiryTime
            };

            await authDbContext.RefreshTokens.AddAsync(token);
            await authDbContext.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var storedToken = authDbContext.RefreshTokens
                .FirstOrDefault(rt => rt.UserId == userId && rt.Token == refreshToken);

            if (storedToken == null) return false;

            if (storedToken.ExpiryTime < DateTime.UtcNow)
            {
                authDbContext.RefreshTokens.Remove(storedToken);
                await authDbContext.SaveChangesAsync();
                return false;
            }

            return true;
        }

        public async Task<string> RefreshTokenAsync(Guid userId, string refreshToken)
        {
            var isValid = await ValidateRefreshTokenAsync(userId, refreshToken);

            if (!isValid) return null;

            var existingToken = await authDbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (existingToken.UsedCount >= 2)
            {
                authDbContext.RefreshTokens.Remove(existingToken);
                await authDbContext.SaveChangesAsync();

                return await CreateRefreshTokenAsync(userId);
            }

            else
            {
                existingToken.UsedCount++;
                authDbContext.RefreshTokens.Update(existingToken);
                await authDbContext.SaveChangesAsync();

                return existingToken.Token;
            }
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var existingToken = await authDbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (existingToken != null)
            {
                authDbContext.RefreshTokens.Remove(existingToken);
                await authDbContext.SaveChangesAsync();
            }
        }        
    }
}
