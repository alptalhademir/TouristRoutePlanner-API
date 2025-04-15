using Microsoft.AspNetCore.Identity;

namespace TouristRoutePlanner.API.Models
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
