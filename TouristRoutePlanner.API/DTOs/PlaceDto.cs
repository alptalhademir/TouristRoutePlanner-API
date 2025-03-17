namespace TouristRoutePlanner.API.DTOs
{
    public class PlaceDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string LanguageCode { get; set; }
        public double Rating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<string> TypeNames { get; set; }
    }
}
