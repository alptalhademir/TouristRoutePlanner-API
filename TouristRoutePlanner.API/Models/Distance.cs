namespace TouristRoutePlanner.API.Models
{
    public class Distance
    {
        public Guid Id { get; set; }

        public string OriginPlaceExternalId { get; set; }
        public Place OriginPlace { get; set; }

        public string DestinationPlaceExternalId { get; set; }
        public Place DestinationPlace { get; set; }

        public string WalkingDistance { get; set; }
        public string WalkingDuration { get; set; }
        public string DrivingDistance { get; set; }
        public string DrivingDuration { get; set; }
    }
}
