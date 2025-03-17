namespace TouristRoutePlanner.API.Models
{
    public class Type
    {
        public Guid id { get; set; }

        public string Name { get; set; }

        public ICollection<PlaceType> PlaceTypes { get; set; }
            = new List<PlaceType>();
    }
}
