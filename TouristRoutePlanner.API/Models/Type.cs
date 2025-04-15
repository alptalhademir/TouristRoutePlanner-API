namespace TouristRoutePlanner.API.Models
{
    public class Type
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<PlaceType> PlaceTypes { get; set; }
            = new List<PlaceType>();

        public ICollection<TravelType> TravelTypes { get; set; }
            = new List<TravelType>();
    }
}
