namespace TouristRoutePlanner.API.Models
{
    public class PlaceType
    {
        public Guid PlaceId { get; set; }
        public Place Place { get; set; }

        public Guid TypeId { get; set; }
        public Type Type { get; set; }
    }
}
