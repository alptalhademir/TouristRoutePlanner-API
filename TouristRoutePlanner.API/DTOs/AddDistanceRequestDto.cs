using System.ComponentModel.DataAnnotations;

namespace TouristRoutePlanner.API.DTOs
{
    public class AddDistanceRequestDto
    {
        [Required]
        public string OriginPlaceExternalId { get; set; }
        [Required]
        public string DestinationPlaceExternalId { get; set; }
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
