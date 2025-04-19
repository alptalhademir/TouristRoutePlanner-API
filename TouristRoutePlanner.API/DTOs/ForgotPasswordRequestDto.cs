using System.ComponentModel.DataAnnotations;

namespace TouristRoutePlanner.API.DTOs
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
