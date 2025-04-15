using System.ComponentModel.DataAnnotations;

namespace TouristRoutePlanner.API.DTOs
{
    public class UpdatePlaceRequestDto
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string LanguageCode { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [Range(0, 5.0)]
        public double Rating { get; set; }

        public string? PriceLevel { get; set; }

        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> Types { get; set; } = new List<string>();
    }
}
