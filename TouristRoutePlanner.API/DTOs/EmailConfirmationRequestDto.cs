using System.ComponentModel.DataAnnotations;

namespace TouristRoutePlanner.API.DTOs
{
    public class EmailConfirmationRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
