namespace TouristRoutePlanner.API.DTOs
{
    public class DistanceDto
    {
        public Guid Id { get; set; }
        public string OriginPlaceExternalId { get; set; }
        public string DestinationPlaceExternalId { get; set; }
        public string WalkingDistance { get; set; }
        public string WalkingDuration { get; set; }
        public string DrivingDistance { get; set; }
        public string DrivingDuration { get; set; }
    }
}
