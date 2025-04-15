using System.ComponentModel.DataAnnotations;

namespace TouristRoutePlanner.API.DTOs
{
    public class UpdateDistanceRequestDto
    {
        [Required]
        public string WalkingDistance { get; set; }
        [Required]
        public string WalkingDuration { get; set; }
        [Required]
        public string DrivingDistance { get; set; }
        [Required]
        public string DrivingDuration { get; set; }
    }
}
