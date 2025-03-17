using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Data
{
    public class TouristRoutePlannerAuthDbContext: IdentityDbContext<User>
    {
        public TouristRoutePlannerAuthDbContext(DbContextOptions<TouristRoutePlannerAuthDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var userRoleId = "dd48cdac-a148-483f-8071-90e349ed476a";
            var adminRoleId = "4fe4aabb-19b3-41ec-bbaa-1efcaf7f9127";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }

    }
}
