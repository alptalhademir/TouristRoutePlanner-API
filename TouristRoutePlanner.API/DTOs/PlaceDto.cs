namespace TouristRoutePlanner.API.DTOs
{
    public class PlaceDto
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public string DisplayName { get; set; }
        public string LanguageCode { get; set; }
        public string City { get; set; }
        public double Rating { get; set; }
        public string? PriceLevel { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public ICollection<string> Types { get; set; } 
            = new List<string>();
    }
}
