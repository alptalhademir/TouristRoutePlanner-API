using System.ComponentModel.DataAnnotations;

namespace TouristRoutePlanner.API.DTOs
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}
