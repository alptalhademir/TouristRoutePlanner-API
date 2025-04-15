namespace TouristRoutePlanner.API.Models
{
    public class TravelPlace
    {
        public Guid TravelId { get; set; }
        public Travel Travel { get; set; }
        public Guid PlaceId { get; set; }
        public Place Place { get; set; }
    }
}
