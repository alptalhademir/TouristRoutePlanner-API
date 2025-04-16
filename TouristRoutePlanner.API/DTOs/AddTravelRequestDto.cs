using System.ComponentModel.DataAnnotations;
using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.DTOs
{
    public class AddTravelRequestDto
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [EnumDataType(typeof(TravelerType))]
        public TravelerType TravelerType { get; set; }

        [Required]
        [MinLength(1)]
        public List<Guid> TypeIds { get; set; } = new();

        [Required]
        [MinLength(1)]
        public List<Guid> PlaceIds { get; set; } = new();
    }
}
