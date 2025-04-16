using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.DTOs
{
    public class TravelDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string City { get; set; }
        public string Title { get; set; }
        public TravelerType TravelerType { get; set; }
        public List<TypeDto> SelectedTypes { get; set; } = new();
        public List<PlaceDto> SelectedPlaces { get; set; } = new();
    }
}
