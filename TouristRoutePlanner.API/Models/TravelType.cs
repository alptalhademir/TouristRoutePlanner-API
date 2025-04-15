namespace TouristRoutePlanner.API.Models
{
    public class TravelType
    {
        public Guid TravelId { get; set; }
        public Travel Travel { get; set; }
        public Guid TypeId { get; set; }
        public Type Type { get; set; }
    }
}
