using System.ComponentModel.DataAnnotations;

namespace TouristRoutePlanner.API.DTOs
{
    public class UpdateUserRequestDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }

        [Compare("NewPassword")]
        public string? ConfirmNewPassword { get; set; }
    }
}
